[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/Cg1EVVDK)
[![Open in Codespaces](https://classroom.github.com/assets/launch-codespace-2972f46106e565e64193e422d61a12cf1da4916b45550586e14ef0a7c637dd04.svg)](https://classroom.github.com/open-in-codespaces?assignment_repo_id=19627410)
# SESION DE LABORATORIO N° 05: PRUEBAS CON MUTACIONES

### Nombre:

## OBJETIVOS
  * Comprender el funcionamiento de las pruebas con mutanciones utilizando Stryker mutator.

## REQUERIMIENTOS
  * Conocimientos: 
    - Conocimientos básicos de Bash (powershell).
    - Conocimientos básicos de Contenedores (Docker).
  * Hardware:
    - Virtualization activada en el BIOS..
    - CPU SLAT-capable feature.
    - Al menos 4GB de RAM.
  * Software:
    - Windows 10 64bit: Pro, Enterprise o Education (1607 Anniversary Update, Build 14393 o Superior)
    - Docker Desktop 
    - Powershell versión 7.x
    - Net 8 o superior
    - Visual Studio Code

## CONSIDERACIONES INICIALES
  * Clonar el repositorio mediante git para tener los recursos necesarios

## DESARROLLO
1. Iniciar la aplicación Powershell o Windows Terminal en modo administrador 
2. Ejecutar el siguiente comando para crear una nueva solución
```
dotnet new sln -o Bank
```
3. Acceder a la solución creada y ejecutar el siguiente comando para crear una nueva libreria de clases y adicionarla a la solución actual.
```
cd Bank
dotnet new classlib -o Bank.Domain
dotnet sln add ./Bank.Domain/Bank.Domain.csproj
```
4. Ejecutar el siguiente comando para crear un nuevo proyecto de pruebas y adicionarla a la solución actual
```
dotnet new xunit -o Bank.Domain.Tests
dotnet sln add ./Bank.Domain.Tests/Bank.Domain.Tests.csproj
dotnet add ./Bank.Domain.Tests/Bank.Domain.Tests.csproj reference ./Bank.Domain/Bank.Domain.csproj
```
5. Iniciar Visual Studio Code (VS Code) abriendo el folder de la solución como proyecto. En el proyecto Bank.Domain, si existe un archivo Class1.cs proceder a eliminarlo. Asimismo en el proyecto Bank.Domain.Tests si existiese un archivo UnitTest1.cs, también proceder a eliminarlo.

6. En VS Code, en el proyecto Bank.Domain proceder a crear el archivo BankAccount.cs e introducir el siguiente código:
```C#
public class BankAccount
{
    public const string DebitAmountExceedsBalanceMessage = "Debit amount exceeds balance";
    public const string DebitAmountLessThanZeroMessage = "Debit amount is less than zero";
    private readonly string m_customerName;
    private double m_balance;
    private BankAccount() { }
    public BankAccount(string customerName, double balance)
    {
        m_customerName = customerName;
        m_balance = balance;
    }
    public string CustomerName { get { return m_customerName; } }
    public double Balance { get { return m_balance; }  }
    public void Debit(double amount)
    {
        if (amount > m_balance)
            throw new ArgumentOutOfRangeException("amount", amount, DebitAmountExceedsBalanceMessage);
        if (amount < 0)
            throw new ArgumentOutOfRangeException("amount", amount, DebitAmountLessThanZeroMessage);
        m_balance -= amount; 
    }
    public void Credit(double amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException("amount");
        m_balance += amount;
    }
}
```
7. Luego en el proyecto Bank.Domain.Tests añadir un nuevo archivo BanckAccountTests.cs e introducir el siguiente código:
```C#
using Bank.Domain;
using System;
using Xunit;

namespace Bank.Domain.Tests;
public class BankAccountTests
{
    [Theory]
    [InlineData(11.99, 4.55, 7.44)]
    [InlineData(12.3, 5.2, 7.1)]
    public void MultiDebit_WithValidAmount_UpdatesBalance(
        double beginningBalance, double debitAmount, double expected )
    {
        // Arrange
        BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);
        // Act
        account.Debit(debitAmount);
        // Assert
        double actual = account.Balance;
        Assert.Equal(Math.Round(expected,2), Math.Round(actual,2));
    }

    [Fact]
    public void Debit_WhenAmountIsLessThanZero_ShouldThrowArgumentOutOfRange()
    {
        // Arrange
        double beginningBalance = 11.99;
        double debitAmount = -100.00;
        BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);
        // Act and assert
        Assert.Throws<System.ArgumentOutOfRangeException>(() => account.Debit(debitAmount));
    }

    [Fact]
    public void Debit_WhenAmountIsMoreThanBalance_ShouldThrowArgumentOutOfRange()
    {
        // Arrange
        double beginningBalance = 11.99;
        double debitAmount = 20.0;
        BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);
        // Act
        try
        {
            account.Debit(debitAmount);
        }
        catch (System.ArgumentOutOfRangeException e)
        {
            // Assert
            Assert.Contains(BankAccount.DebitAmountExceedsBalanceMessage, e.Message);
        }
    }
}
```
8. Abrir un terminal en VS Code (CTRL + Ñ) o vuelva al terminal anteriormente abierto, y ejecutar los comandos:
```Bash
dotnet test --collect:"XPlat Code Coverage"
```
9. El paso anterior debe producir un resultado satisfactorio como el siguiente. 
```Bash
Failed!  - Failed:     0, Passed:     4, Skipped:     0, Total:     1, Duration: < 1 ms
```
10. En el terminal, proceder a ejecutar los siguientes comandos para realizar pruebas de mutacion
```Bash
dotnet tool install -g dotnet-stryker
dotnet stryker
```
11. El paso anterior debe producir un resultado satisfactorio similar al siguiente. 
```Bash
   _____ _              _               _   _ ______ _______  
  / ____| |            | |             | \ | |  ____|__   __| 
 | (___ | |_ _ __ _   _| | _____ _ __  |  \| | |__     | |    
  \___ \| __| '__| | | | |/ / _ \ '__| | . ` |  __|    | |    
  ____) | |_| |  | |_| |   <  __/ |    | |\  | |____   | |    
 |_____/ \__|_|   \__, |_|\_\___|_| (_)|_| \_|______|  |_|    
                   __/ |                                      
                  |___/                                       


Version: 4.3.0

[08:42:56 INF] Analysis starting.
[08:42:56 INF] Identifying projects to mutate in C:\Users\HP\source\repos\Bank\Bank.sln. This can take a while.
[08:42:57 INF] Found project C:\Users\HP\source\repos\Bank\Bank.Domain\Bank.Domain.csproj to mutate.
[08:42:57 INF] Analysis complete.
[08:42:57 INF] Building solution Bank.sln
[08:42:57 INF] Building project Bank.sln using dotnet build Bank.sln (directory C:\Users\HP\source\repos\Bank.)
[08:43:00 INF] Number of tests found: 4 for project C:\Users\HP\source\repos\Bank\Bank.Domain\Bank.Domain.csproj. Initial test run started.
[08:43:02 INF] 22 mutants created
[08:43:02 INF] Capture mutant coverage using 'CoverageBasedTest' mode.
Hint: by passing "--open-report or -o" the report will open automatically and update the report in real-time.
[08:43:02 INF] 7     mutants got status NoCoverage.   Reason: Not covered by any test.
[08:43:02 INF] 2     mutants got status Ignored.      Reason: Removed by block already covered filter
[08:43:02 INF] 9     total mutants are skipped for the above mentioned reasons
[08:43:02 INF] 13    total mutants will be tested
██████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████100,00% │ Testing mutant 13 / 13 │ K 8 │ S 5 │ T 0 │ ~0m 00s │                                                                                                                                                    00:00:16

Killed:    8
Survived:  5
Timeout:   0

Your html report has been generated at:
C:\Users\HP\source\repos\Bank\StrykerOutput\2024-10-26.08-42-55\reports\mutation-report.html
You can open it in your browser of choice.
```

12. Finalmente proceder a verificar la cobertura, dentro del proyecto Primes.Tests se dede haber generado una carpeta o directorio TestResults, en el cual posiblemente exista otra subpcarpeta o subdirectorio conteniendo un archivo con nombre `coverage.cobertura.xml`, si existe ese archivo proceder a ejecutar los siguientes comandos desde la linea de comandos abierta anteriomente, de los contrario revisar el paso 8:
```
dotnet tool install -g dotnet-reportgenerator-globaltool
ReportGenerator "-reports:./*/*/*/coverage.cobertura.xml" "-targetdir:Cobertura" -reporttypes:HTML
```
13. El comando anterior primero proceda instalar una herramienta llamada ReportGenerator (https://reportgenerator.io/) la cual mediante la segunda parte del comando permitira generar un reporte en formato HTML con la cobertura obtenida de la ejecución de las pruebas. Este reporte debe localizarse dentro de una carpeta llamada Cobertura y puede acceder a el abriendo con un navegador de internet el archivo index.htm.

---
## Actividades Encargadas
1. Adicionar los escenarios, casos de prueba, metodos de prueba para las mutaciones no coberturadas (7) e ignoradas (2) para completar la cobertura del 100% del código.
2. Completar la documentación del Clases, atributos y métodos para luego generar una automatización (publish_docs.yml) que genere la documentación utilizando DocFx y la publique en una Github Page
3. Generar una automatización (publish_cov_report.yml) que: * Compile el proyecto y ejecute las pruebas unitarias, * Genere el reporte de cobertura, * Publique el reporte en Github Page
4. Generar una automatización (publish_mut_report.yml) que: * compile el proyecto y ejecute las pruebas del código * Genere el reporte de mutaciones (Striker) * Publique el reporte en Github Page.
5. Generar una automatización (release.yml) que: * Genere el nuget con su codigo de matricula como version del componente, * Publique el nuget en Github Packages, * Genere el release correspondiente
