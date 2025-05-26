using Bank.Domain; // Asegúrate de que este using esté presente
using System;
using Xunit;

namespace Bank.Domain.Tests;

public class BankAccountTests
{
    // TUS PRUEBAS EXISTENTES (DEJA ESTAS COMO ESTÁN INICIALMENTE)
    [Theory]
    [InlineData(11.99, 4.55, 7.44)]
    [InlineData(12.3, 5.2, 7.1)]
    public void MultiDebit_WithValidAmount_UpdatesBalance(
        double beginningBalance, double debitAmount, double expected)
    {
        // Arrange
        BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);
        // Act
        account.Debit(debitAmount);
        // Assert
        double actual = account.Balance;
        Assert.Equal(Math.Round(expected, 2), Math.Round(actual, 2));
    }

    [Fact]
    public void Debit_WhenAmountIsLessThanZero_ShouldThrowArgumentOutOfRange()
    {
        // Arrange
        double beginningBalance = 11.99;
        double debitAmount = -100.00;
        BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);
        // Act and assert
        var exception = Assert.Throws<System.ArgumentOutOfRangeException>(() => account.Debit(debitAmount));
        Assert.Equal("amount", exception.ParamName); // Buena práctica verificar el ParamName
        Assert.Contains(BankAccount.DebitAmountLessThanZeroMessage, exception.Message); // Verificar el mensaje específico
    }

    [Fact]
    public void Debit_WhenAmountIsMoreThanBalance_ShouldThrowArgumentOutOfRange()
    {
        // Arrange
        double beginningBalance = 11.99;
        double debitAmount = 20.0;
        BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

        // Act and assert
        var exception = Assert.Throws<System.ArgumentOutOfRangeException>(() => account.Debit(debitAmount));
        Assert.Equal("amount", exception.ParamName);
        Assert.Contains(BankAccount.DebitAmountExceedsBalanceMessage, exception.Message);
    }

    // ---------------------------------------------------------------------------
    // --- NUEVAS PRUEBAS Y MODIFICACIONES PARA ABORDAR MUTANTES NO CUBIERTOS Y SUPERVIVIENTES ---
    // ---------------------------------------------------------------------------

    // --- PRUEBAS PARA "NO COVERAGE" ---

    [Fact]
    public void CustomerName_ReturnsCorrectName_AfterConstruction()
    {
        // ARREGLA: NoCoverage en el get de CustomerName (línea 13 del reporte)
        // Arrange
        string expectedName = "John Doe";
        var account = new BankAccount(expectedName, 100.00);

        // Act
        string actualName = account.CustomerName;

        // Assert
        Assert.Equal(expectedName, actualName);
    }

    [Fact]
    public void Constructor_SetsInitialBalanceCorrectly()
    {
        // ARREGLA: NoCoverage en el get de Balance (línea 14 del reporte)
        // (Aunque otras pruebas usan Balance, esta verifica explícitamente el estado inicial)
        // Arrange
        double expectedBalance = 123.45;
        var account = new BankAccount("Jane Doe", expectedBalance);

        // Act
        double actualBalance = account.Balance;

        // Assert
        Assert.Equal(expectedBalance, actualBalance);
    }

    [Fact]
    public void Credit_WhenAmountIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
    {
        // ARREGLA: NoCoverage en la validación de amount < 0 en Credit (líneas 25-26 del reporte)
        // Arrange
        var account = new BankAccount("Test User", 100.00);
        double creditAmount = -50.00;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => account.Credit(creditAmount));
        Assert.Equal("amount", exception.ParamName); // Verifica que el ParamName es "amount"
    }

    [Theory]
    [InlineData(100.00, 50.00, 150.00)]
    [InlineData(0.00, 25.50, 25.50)]
    public void Credit_WithValidPositiveAmount_UpdatesBalanceCorrectly(double beginningBalance, double creditAmount, double expectedBalance)
    {
        // ARREGLA: NoCoverage en m_balance += amount; en Credit (línea 27 del reporte)
        //          y también puede matar mutantes 'Survived' si cambian '+='
        // Arrange
        var account = new BankAccount("Test User", beginningBalance);

        // Act
        account.Credit(creditAmount);

        // Assert
        Assert.Equal(expectedBalance, account.Balance, 2); // Usar precisión para doubles
    }

    [Fact]
    public void Credit_WhenAmountIsZero_ShouldNotChangeBalance()
    {
        // ARREGLA: Posible NoCoverage o Survived en Credit si Stryker muta `< 0` a `<= 0`
        // (Cubre el caso límite de `amount == 0` para `Credit`)
        // Arrange
        double beginningBalance = 100.00;
        var account = new BankAccount("Test User", beginningBalance);
        double creditAmount = 0.00;

        // Act
        account.Credit(creditAmount); // No debería lanzar excepción

        // Assert
        Assert.Equal(beginningBalance, account.Balance, 2);
    }


    // --- PRUEBAS PARA "SURVIVED" ---

    [Fact]
    public void Debit_WhenAmountIsEqualToBalance_ShouldUpdateBalanceToZero()
    {
        // ARREGLA: Survived en `if (amount > m_balance)` (línea 17),
        //          si Stryker lo cambia a `amount >= m_balance`.
        //          Tu código actual permite debitar el saldo exacto.
        // Arrange
        double beginningBalance = 50.00;
        double debitAmount = 50.00;
        BankAccount account = new BankAccount("Edge Case Debitor", beginningBalance);

        // Act
        account.Debit(debitAmount); // No debería lanzar ArgumentOutOfRangeException por "exceeds balance"

        // Assert
        Assert.Equal(0, account.Balance, 2);
    }

    [Fact]
    public void Debit_WhenAmountIsZero_ShouldNotChangeBalance()
    {
        // ARREGLA: Survived en `if (amount < 0)` (línea 19),
        //          si Stryker lo cambia a `amount <= 0`.
        //          Un débito de 0 es válido y no debería lanzar excepción.
        // Arrange
        double beginningBalance = 50.00;
        double debitAmount = 0.00;
        BankAccount account = new BankAccount("Zero Debitor", beginningBalance);

        // Act
        account.Debit(debitAmount); // No debería lanzar ninguna excepción

        // Assert
        Assert.Equal(beginningBalance, account.Balance, 2);
    }
}