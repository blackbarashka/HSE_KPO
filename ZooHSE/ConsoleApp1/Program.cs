using System;
using Xunit;
using Moq;
using ZooHSE;
using ZooHSE.Animals;
using Microsoft.Extensions.DependencyInjection;
using ZooHSE.Things;

public class ZooTests
{
    private readonly Zoo _zoo;
    private readonly Mock<VetClinic> _mockVetClinic;

    public ZooTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<Zoo>()
            .BuildServiceProvider();

        _zoo = serviceProvider.GetService<Zoo>();
        _mockVetClinic = new Mock<VetClinic>();
    }

    [Fact]
    public void AddAnimal_ShouldAddHerbo()
    {
        // Arrange
        var herbo = new Herbo(5, 1,"Ovn", true, "Deer", 7);

        // Act
        _zoo.AddAnimal(herbo, _mockVetClinic.Object);

        // Assert
        Assert.Contains(herbo, _zoo.GetAnimals());
    }

    [Fact]
    public void AddAnimal_ShouldAddPredator()
    {
        // Arrange
        var predator = new Predator(10, 1, "Simba", true, "Lion" );

        // Act
        _zoo.AddAnimal(predator, _mockVetClinic.Object);

        // Assert
        Assert.Contains(predator, _zoo.GetAnimals());
    }

    [Fact]
    public void AddInventoryItem_ShouldAddTable()
    {
        // Arrange
        var table = new Table(1, "Стол");

        // Act
        _zoo.AddInventoryItem(table);

        // Assert
        Assert.Contains(table, _zoo.GetInventory());
    }

    [Fact]
    public void AddInventoryItem_ShouldAddComputer()
    {
        // Arrange
        var computer = new Computer(1, "Компьютер");

        // Act
        _zoo.AddInventoryItem(computer);

        // Assert
        Assert.Contains(computer, _zoo.GetInventory());
    }

    [Fact]
    public void PrintFoodRequirements_ShouldPrintRequirements()
    {
        // Arrange
        var herbo = new Herbo(5, 1, "Ovn", true, "Deer", 7);
        var predator = new Predator(10, 1, "Simba", true, "Lion");
        _zoo.AddAnimal(herbo, _mockVetClinic.Object);
        _zoo.AddAnimal(predator, _mockVetClinic.Object);

        // Act
        var output = CaptureConsoleOutput(() => _zoo.PrintFoodRequirements());

        // Assert
        Assert.Contains("Deer requires 5 kg of food per day.", output);
        Assert.Contains("Lion requires 10 kg of food per day.", output);
    }

    private string CaptureConsoleOutput(Action action)
    {
        var output = new StringWriter();
        Console.SetOut(output);
        action();
        return output.ToString();
    }
}
