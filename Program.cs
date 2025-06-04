using System;
using System.Data.Entity; // This is needed for DbContext and DbSet
using System.Linq;      // This is needed for .ToList()

// 1. Define your "Student" (POCO - Plain Old C# Object)
// This class represents a row in your Student database table.
public class Student
{
    public int StudentId { get; set; } // Entity Framework will automatically make this the primary key
    public string StudentName { get; set; } // The student's name
    // You could add more properties here like:
    // public DateTime DateOfBirth { get; set; }
    // public string Email { get; set; }
}

// 2. Define your "DbContext"
// This class is the main bridge between your C# code and the database.
public class SchoolContext : DbContext
{
    // Constructor: Tells DbContext which connection string to use from App.config
    public SchoolContext() : base("name=SchoolDBConnectionString")
    {
        // This line ensures the database is created if it doesn't exist,
        // or updated if your model changes (for development purposes).
        Database.SetInitializer(new CreateDatabaseIfNotExists<SchoolContext>());
    }

    // DbSet: Represents a table in your database.
    // In this case, it will create a table named "Students" for your "Student" objects.
    public DbSet<Student> Students { get; set; }
}

// 3. Main Application Logic
// This is where your program starts and runs.
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting Entity Framework Code-First application...");

        try
        {
            // Create an instance of our DbContext.
            // The 'using' ensures the database connection is properly closed.
            using (var context = new SchoolContext())
            {
                // This line will trigger the database creation if it doesn't exist
                // based on your SchoolContext and Student model.
                // It's good practice to call it explicitly, though EF often does it automatically.
                context.Database.Initialize(force: true); // Ensures the initializer runs
                Console.WriteLine("Database 'SchoolDB' ensured/created successfully.");

                // Create a new Student object
                var newStudent = new Student() { StudentName = "Alice Wonderland" };

                // Add the new student to the 'Students' DbSet (which represents the table)
                context.Students.Add(newStudent);

                // Save the changes to the database. This is when the student is actually inserted.
                context.SaveChanges();
                Console.WriteLine($"Student '{newStudent.StudentName}' added successfully with ID: {newStudent.StudentId}");

                // --- Optional: Verify the student was added by retrieving all students ---
                Console.WriteLine("\n--- Students currently in the database ---");
                var studentsInDb = context.Students.ToList(); // Get all students from the database

                if (studentsInDb.Any()) // Check if there are any students
                {
                    foreach (var s in studentsInDb)
                    {
                        Console.WriteLine($"ID: {s.StudentId}, Name: {s.StudentName}");
                    }
                }
                else
                {
                    Console.WriteLine("No students found in the database (this shouldn't happen after adding one!).");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nAn ERROR occurred:");
            Console.WriteLine(ex.Message); // Shows a general error message
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Details: " + ex.InnerException.Message); // More specific error
            }
            Console.WriteLine("Please check your Visual Studio output for more details.");
        }

        Console.WriteLine("\nPress any key to exit the application...");
        Console.ReadKey(); // Keeps the console window open until you press a key
    }
}