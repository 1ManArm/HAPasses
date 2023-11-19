using System;
using System.Collections.Generic;
using System.Linq;

namespace HAPasses
{
    internal class Program
    {
        public class Employee
        {
            public int EmployeeId { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Position { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public List<IPass> Passes { get; set; } = new List<IPass>();
        }
        public interface IPass
        {
            bool Validate();
            void PrintInfo();
        }
        public class TemporaryPass : IPass
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            public bool Validate()
            {
                return DateTime.Now >= StartDate && DateTime.Now <= EndDate;
            }

            public void PrintInfo()
            {
                Console.WriteLine($"Temporary Pass: {StartDate} - {EndDate}");
            }
        }
        public class PermanentPass : IPass
        {
            public bool Validate()
            {
                return true;
            }

            public void PrintInfo()
            {
                Console.WriteLine("Permanent Pass");
            }
        }
        public class PassSystem
        {
            public List<Employee> employees = new List<Employee>();

            public delegate void LogHandler(string message);
            public event LogHandler Logged;

            public void RegisterEmployee(Employee employee)
            {
                employees.Add(employee);
                Logged?.Invoke($"Employee {employee.FirstName} {employee.LastName} has been registered.");
            }

            public void IssuePass(Employee employee, IPass pass)
            {
                Logged?.Invoke($"Pass has been issued to employee {employee.FirstName} {employee.LastName}.");
            }

            public void AccessControl(Employee employee, IPass pass)
            {
                if (pass.Validate())
                {
                    Logged?.Invoke($"Access granted to employee {employee.FirstName} {employee.LastName}.");
                }
                else
                {
                    Logged?.Invoke($"Access denied to employee {employee.FirstName} {employee.LastName}.");
                }
            }

            public void PrintEmployeeList()
            {
                foreach (var employee in employees)
                {
                    Console.WriteLine($"{employee.EmployeeId} - {employee.FirstName} {employee.LastName} - {employee.Position}");
                }
            }

            public IEnumerable<Employee> GetEmployeesWithPermanentPass()
            {
                return employees.Where(e => e.Passes.OfType<PermanentPass>().Any());
            }

            public IEnumerable<Employee> GetEmployeesWithExpiringTemporaryPass(DateTime currentDate)
            {
                return employees.Where(e => e.Passes.OfType<TemporaryPass>().Any(p => p.EndDate.Date == currentDate.Date));
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            PassSystem passSystem = new PassSystem();

            // Регистрация нового сотрудника
            Employee employee1 = new Employee
            {
                EmployeeId = 1,
                LastName = "Smith",
                FirstName = "John",
                Position = "Manager",
                Login = "jsmith",
                Password = "12345"
            };
            passSystem.RegisterEmployee(employee1);

            // Выдача пропуска
            TemporaryPass tempPass = new TemporaryPass
            {
                StartDate = new DateTime(2022, 5, 1),
                EndDate = new DateTime(2022, 5, 15)
            };
            passSystem.IssuePass(employee1, tempPass);

            // Проверка доступа
            passSystem.AccessControl(employee1, tempPass);

            // Вывод списка сотрудников
            passSystem.PrintEmployeeList();

            // Получение списка сотрудников с постоянным пропуском
            var employeesWithPermanentPass = passSystem.GetEmployeesWithPermanentPass();
            foreach (var emp in employeesWithPermanentPass)
            {
                Console.WriteLine($"{emp.FirstName} {emp.LastName} has a permanent pass.");
            }

            // Получение списка сотрудников с просроченным временным пропуском
            var currentDate = DateTime.Now;
            var employeesWithExpiringTemporaryPass = passSystem.GetEmployeesWithExpiringTemporaryPass(currentDate);
            foreach (var emp in employeesWithExpiringTemporaryPass)
            {
                Console.WriteLine($"{emp.FirstName} {emp.LastName} has a temporary pass expiring today.");
            }
        }
    }
}