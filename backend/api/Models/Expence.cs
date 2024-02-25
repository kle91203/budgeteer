
namespace Edwards.Kevin.Budgeteer.Models;

public class Expence 
{
    public int Id {get; init; }
    public int Amount {get; init; }
    public Category Category {get; init; }
    public string Description {get; init; }

    public Expence(int id, int amount, Category category, string description)
    //amount is int because it's easier to do ints and covert to a float to render in the ui
    {
        Id = id;
        Amount = amount;
        Category = category;
        Description = description;
    }
}

public enum Category 
{
    Groceries, Gas_Fuel, Gas_Bill, Electricity, CellPhones, Mortgage, Water, Clothing, Shopping, Restauraunts
}