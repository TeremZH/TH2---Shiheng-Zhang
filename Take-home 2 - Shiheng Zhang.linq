<Query Kind="Statements">
  <Connection>
    <ID>5388075e-d0ce-41fd-96ff-ad0efe5cfd93</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>TEREM</Server>
    <Database>WestWind-2024</Database>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

//Q2
Customers
    .Where(c => c.Orders.Count >= 1 && c.Orders.Count < 5)
    .OrderBy(c => c.City)
    .Select(c => new
    {
        Customer = c.CompanyName,
        c.City,
        Orders = c.Orders
            .OrderByDescending(o => o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)))
            .Select(o => new
            {
                OrderID = o.OrderID, 
                Employee = $"{o.Employee.FirstName} {o.Employee.LastName}", 
                OrderDate = o.OrderDate, 
                Total = o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)) 
            })
            .ToList()
    })
    .Dump();
	
//Q3
Suppliers
    .Select(s => new 
    {
        Company = s.CompanyName,
        Products = s.Products
            .Where(p => !p.Discontinued && p.UnitsInStock < p.ReorderLevel)
            .OrderBy(p => p.ProductName)
            .Select(p => new
            {
                Name = p.ProductName,
                QtyPerUnit = p.QuantityPerUnit,
                ReorderLevel = p.ReorderLevel,
                InStock = p.UnitsInStock,
                QtyToOrder = p.ReorderLevel - p.UnitsInStock
            })
            .ToList()
    })
    .Where(s => s.Products.Count > 1) 
    .OrderBy(s => s.Company) 
    .Dump();

//Q4
Products
    .GroupBy(p => new 
    { 
        Category = p.Category.CategoryName, 
        Supplier = p.Supplier.CompanyName 
    })
    .OrderBy(g => g.Key.Category)
    .ThenBy(g => g.Key.Supplier)
    .Select(g => new
    {
        g.Key.Category,
        g.Key.Supplier,
        Count = g.Count(),
        AveragePrice = g.Average(p => p.UnitPrice),
        MinPrice = g.Min(p => p.UnitPrice),
        MaxPrice = g.Max(p => p.UnitPrice),
        OnHandValue = g.Sum(p => p.UnitPrice * p.UnitsInStock)
    })
    .Dump();

//Q5
Orders
    .Where(o => o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity) > 11000)
    .OrderBy(o => o.Customer.Country)
    .ThenBy(o => o.Customer.City)
    .Select(o => new
    {
    	Customer = o.Customer.CompanyName,
        City = o.Customer.City,
        Country = o.Customer.Country,
        Amount = o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity),
        Detail = o.OrderDetails
            .OrderBy(od => od.Product.ProductName) 
            .Select(od => new 
            {
                Product = od.Product.ProductName,
                Qty = od.Quantity,
                Cost = od.UnitPrice
            })
            .ToList()
    })
    .Dump();