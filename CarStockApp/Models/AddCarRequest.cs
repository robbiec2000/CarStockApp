﻿namespace CarStockApp;

public class AddCarRequest
{
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Dealer { get; set; }
}