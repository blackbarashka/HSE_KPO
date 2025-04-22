using Xunit;
using Moq;
using ZooHSE;
using ZooHSE.Animals;
using ZooHSE.Things;

/// <summary>
/// Тесты для класса Zoo.
/// </summary>
public class ZooTests
{
    private readonly Zoo _zoo;
    private readonly Mock<VetClinic> _mockVetClinic;

    public ZooTests()
    {
        _mockVetClinic = new Mock<VetClinic>();
        _zoo = new Zoo();
    }

    /// <summary>
    /// Тест на добавление травоядного животного.
    /// </summary>
    [Fact]
    public void AddAnimal_ShouldAddHerbivoreAnimal()
    {
        // Arrange
        var herbo = new Herbo(5, 1, "Мага", true, "Кролик", 7);

        // Act
        _zoo.AddAnimal(herbo, _mockVetClinic.Object);

        // Assert
        Assert.Contains(herbo, _zoo.GetAnimals());
    }

    /// <summary>
    /// Тест на добавление хищного животного.
    /// </summary>
    [Fact]
    public void AddAnimal_ShouldAddPredatorAnimal()
    {
        // Arrange
        var predator = new Predator(10, 2, "Simba", true, "Тигр");

        // Act
        _zoo.AddAnimal(predator, _mockVetClinic.Object);

        // Assert
        Assert.Contains(predator, _zoo.GetAnimals());
    }

    /// <summary>
    /// Тест на добавление предмета в инвентарь.
    /// </summary>
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

    /// <summary>
    /// Тест на добавление компьютера в инвентарь.
    /// </summary>
    [Fact]
    public void AddInventoryItem_ShouldAddComputer()
    {
        // Arrange
        var computer = new Computer(2, "Компьютер");

        // Act
        _zoo.AddInventoryItem(computer);

        // Assert
        Assert.Contains(computer, _zoo.GetInventory());
    }

    /// <summary>
    /// Тест на вывод потребляемой еды в день.
    /// </summary>
    [Fact]
    public void PrintFoodRequirements_ShouldPrintTotalFood()
    {
        // Arrange
        var herbo = new Herbo(5, 1, "Мага", true, "Кролик", 7);
        var predator = new Predator(10, 2, "Simba", true, "Тигр");
        _zoo.AddAnimal(herbo, _mockVetClinic.Object);
        _zoo.AddAnimal(predator, _mockVetClinic.Object);
        // Act
        _zoo.PrintFoodRequirements();
        // Assert
        Assert.Equal(15, _zoo.GetAnimals().Sum(a => a.Food));
    }
    /// <summary>
    /// Тест на вывод животных, которые могут быть добавлены в контактный центр.
    /// </summary>
    [Fact]
    public void PrintContactZooAnimals_ShouldPrintFriendlyAnimals()
    {
        // Arrange
        var herbo = new Herbo(5, 1, "Мага", true, "Кролик", 7);
        var predator = new Predator(10, 2, "Simba", true, "Тигр");
        _zoo.AddAnimal(herbo, _mockVetClinic.Object);
        _zoo.AddAnimal(predator, _mockVetClinic.Object);
        // Act
        _zoo.PrintContactZooAnimals();
        // Assert
        Assert.Contains(herbo, _zoo.GetAnimals());
    }
    /// <summary>
    /// Тест на вывод списка инвертаря в зоопарке.
    /// </summary>
    [Fact]
    public void PrintInventory_ShouldPrintInventory()
    {
        // Arrange
        var herbo = new Herbo(5, 1, "Мага", true, "Кролик", 7);
        var predator = new Predator(10, 2, "Simba", true, "Тигр");
        var table = new Table(1, "Стол");
        var computer = new Computer(2, "Компьютер");
        _zoo.AddAnimal(herbo, _mockVetClinic.Object);
        _zoo.AddAnimal(predator, _mockVetClinic.Object);
        _zoo.AddInventoryItem(table);
        _zoo.AddInventoryItem(computer);
        // Act
        _zoo.PrintInventory();
        // Assert
        Assert.Contains(herbo, _zoo.GetAnimals());
        Assert.Contains(table, _zoo.GetInventory());
    }


}
