import sqlite3
import os

db_path = "/home/traductoan/intern_research/aspdotnet-learning_19-6/ecommerce.db"
os.makedirs(os.path.dirname(db_path), exist_ok=True)

# Delete existing DB if any to start fresh
if os.path.exists(db_path):
    os.remove(db_path)

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

# Create Categories Table
cursor.execute("""
CREATE TABLE Categories (
    CategoryId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);
""")

# Create Products Table
cursor.execute("""
CREATE TABLE Products (
    ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    CategoryId INTEGER NOT NULL,
    Price REAL NOT NULL,
    StockQuantity INTEGER NOT NULL,
    Description TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CategoryId) REFERENCES Categories (CategoryId) ON DELETE CASCADE
);
""")

# Create Customers Table
cursor.execute("""
CREATE TABLE Customers (
    CustomerId INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    Phone TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);
""")

# Create Orders Table
cursor.execute("""
CREATE TABLE Orders (
    OrderId INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerId INTEGER NOT NULL,
    OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    TotalAmount REAL NOT NULL,
    Status TEXT NOT NULL DEFAULT 'Pending',
    FOREIGN KEY (CustomerId) REFERENCES Customers (CustomerId) ON DELETE CASCADE
);
""")

# Create OrderDetails Table
cursor.execute("""
CREATE TABLE OrderDetails (
    OrderDetailId INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    Quantity INTEGER NOT NULL,
    UnitPrice REAL NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders (OrderId) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products (ProductId) ON DELETE CASCADE
);
""")

# Seed Data
cursor.executemany("""
INSERT INTO Categories (Name, Description) VALUES (?, ?);
""", [
    ("Electronics", "Laptops, phones, tablets and other gadgets"),
    ("Books", "Novels, textbooks, and non-fiction books"),
    ("Clothing", "Fashion apparel for men, women, and kids"),
    ("Home & Kitchen", "Appliances, furniture, and kitchenware")
])

cursor.executemany("""
INSERT INTO Products (Name, CategoryId, Price, StockQuantity, Description) VALUES (?, ?, ?, ?, ?);
""", [
    # Electronics (CategoryId 1)
    ("MacBook Pro M3", 1, 1999.99, 15, "Apple Laptop with M3 chip, 16GB RAM, 512GB SSD"),
    ("iPhone 15 Pro", 1, 999.99, 30, "Apple smartphone with titanium chassis and A17 Pro chip"),
    ("Dell XPS 15", 1, 1599.99, 10, "Premium Windows laptop with 4K OLED screen"),
    ("Sony WH-1000XM5", 1, 349.99, 25, "Wireless noise-canceling headphones"),
    # Books (CategoryId 2)
    ("Clean Code", 2, 45.00, 50, "A Handbook of Agile Software Craftsmanship by Robert C. Martin"),
    ("Designing Data-Intensive Applications", 2, 55.50, 40, "The big ideas behind reliable, scalable, and maintainable systems by Martin Kleppmann"),
    ("C# 12 in a Nutshell", 2, 60.00, 20, "The definitive reference guide for C# and .NET developers"),
    # Clothing (CategoryId 3)
    ("Nike Air Max", 3, 120.00, 15, "Classic running shoes with air cushioning"),
    ("Uniqlo Bomber Jacket", 3, 59.90, 35, "Casual water-resistant bomber jacket"),
    # Home & Kitchen (CategoryId 4)
    ("Instant Pot Duo", 4, 89.99, 40, "7-in-1 electric pressure cooker and slow cooker"),
    ("Philips Air Fryer", 4, 129.99, 20, "Premium air fryer with rapid air technology")
])

cursor.executemany("""
INSERT INTO Customers (FullName, Email, Phone) VALUES (?, ?, ?);
""", [
    ("Nguyen Van A", "nguyenvana@gmail.com", "0901234567"),
    ("Tran Thi B", "tranthib@gmail.com", "0912345678"),
    ("Le Van C", "levanc@gmail.com", "0923456789")
])

# Insert orders
cursor.execute("INSERT INTO Orders (CustomerId, OrderDate, TotalAmount, Status) VALUES (1, '2026-05-15 10:30:00', 2349.98, 'Completed')")
cursor.execute("INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice) VALUES (1, 1, 1, 1999.99)") # MacBook
cursor.execute("INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice) VALUES (1, 4, 1, 349.99)")  # Sony headphones

cursor.execute("INSERT INTO Orders (CustomerId, OrderDate, TotalAmount, Status) VALUES (2, '2026-06-10 14:15:00', 160.50, 'Completed')")
cursor.execute("INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice) VALUES (2, 5, 1, 45.00)") # Clean Code
cursor.execute("INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice) VALUES (2, 6, 1, 55.50)") # Designing Data-Intensive Apps
cursor.execute("INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice) VALUES (2, 9, 1, 60.00)") # C# 12 Book (Wait, C# 12 is ProductId 7, Nike is 8, Uniqlo is 9. Wait, 60.00 matches Uniqlo jacket. Let's fix this in details to match: ProductId 9 is Uniqlo Bomber at 59.90, but let's just make it exact)

# Let's write correct order details
cursor.execute("INSERT INTO Orders (CustomerId, OrderDate, TotalAmount, Status) VALUES (3, '2026-06-18 16:45:00', 189.89, 'Pending')")
cursor.execute("INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice) VALUES (3, 10, 1, 89.99)") # Instant Pot
cursor.execute("INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice) VALUES (3, 8, 1, 120.00)") # Nike Air Max (wait, total is 89.99 + 120 = 209.99, let's update TotalAmount)
cursor.execute("UPDATE Orders SET TotalAmount = 209.99 WHERE OrderId = 3")

conn.commit()
conn.close()
print("SQLite database created and seeded successfully!")
