# PerformPlus Payroll & Employee Management System  

**PerformPlus** is a **WPF-based payroll automation and employee management system** designed to simplify salary calculations, track overtime, and manage employee data efficiently.  
Built with **C#** and **MSSQL**, it provides a simple, direct data connection approach no ORM or dependency injection used.

---

## Features  
- Employee registration and management (CRUD operations)  
- Automatic payroll generation (base salary, overtime, taxes, allowances)  
- Admin control panel for employee data updates  
- Real-time validation using `IDataErrorInfo`  
- Configurable defaults (meal, travel, and bonus allowances)  

---

## Database Design  

The system uses a relational SQL Server database that connects directly through C# ADO.NET.  

**Main Tables Include:**  
- `Employees` → Stores personal and salary data  
- `Payrolls` → Contains payroll records, taxes, and overtime  
- `PayrollDefaults` → Stores default allowance and bonus values

## Author  
**Onur Başkan**  
[onurbaskan419@gmail.com](mailto:onurbaskan419@gmail.com)  
[GitHub Profile](https://github.com/BBaskan)

## Tech Stack  
- **Language:** C# (.NET 6)  
- **Framework:** WPF (MVVM pattern)  
- **Database:** MSSQL  
- **Validation:** IDataErrorInfo  
- **UI Design:** MaterialDesignThemes 5.2.1  
