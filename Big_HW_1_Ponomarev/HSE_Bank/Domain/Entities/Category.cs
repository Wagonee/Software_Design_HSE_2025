﻿namespace HSE_Bank.Domain.Entities;

public enum TypeCategory
{
    Income,
    Expense
}

public class Category
{
    public int Id {get; set;}
    public string Name {get; set;}
    public TypeCategory Type {get; set;}

    internal Category(int id, string name, TypeCategory type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Type: {Type}";
    }
}