
// ReSharper disable CheckNamespace
namespace Edwards.Kevin.Budgeteer.Models;

public class Expense(int id, int amount, Category category, string description)
{
    public int Id {get; init; } = id;
    public int Amount {get; init; } = amount;
    public Category Category {get; init; } = category;
    public string Description {get; init; } = description;

    //amount is int because it's easier to do ints and covert to a float to render in the ui
}

public enum Category 
{
    Groceries, 
    //Gas_Fuel, Gas_Bill, 
    Electricity, 
    //CellPhones, Mortgage, Water, Clothing, 
    Shopping, Restaurants
}