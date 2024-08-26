using GPACalculator.Data;
using GPACalculator.Models;
using System.Data;
using System.Data.SqlClient;
namespace GPACalculator.Services
{
    public class DbManager
    {
        SqlConnection connection;
        SqlCommand command;
        public DbManager()
        {
            connection = new SqlConnection();
            connection.ConnectionString = @"Data Source=ZHANIIK\SQLSERVER;
                                            Initial Catalog=GPACalculatorBase;Integrated Security=True;";
            command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
        }
        public double CalculateGPA(List<Course> courses)
        {
            var courseDict = GetAvailableCourses();
            double totalPoints = 0;
            int totalCredits = 0;
            connection.Open();
            try
            {
                foreach (var course in courses)
                {
                    if (courseDict.ContainsKey(course.Name))
                    {
                        int credits = courseDict[course.Name];
                        double gradePoint = StaticData.GradePoints[course.Grade];
                        totalPoints += gradePoint * credits;
                        totalCredits += credits;
                        SqlCommand command = new SqlCommand("UPDATE Courses1 SET grade = @grade WHERE name_of_courses = @name_of_courses", connection);
                        {
                            command.Parameters.AddWithValue("@name_of_courses", course.Name);
                            command.Parameters.AddWithValue("@grade", course.Grade);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid course name: {course.Name}");
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            return totalCredits == 0 ? 0 : Math.Round(totalPoints / totalCredits, 3);
        }
        public string GetNameOfCoursesData()
        {
            var result = new List<string>();
            try
            {
                connection.Open();
                command.CommandText = "SELECT name_of_courses, credits FROM Courses1";
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string courseName = reader["name_of_courses"].ToString()!;
                        int credits = reader.GetInt32(reader.GetOrdinal("credits"));
                        result.Add($"Course: {courseName}, Credits: {credits}");
                    }
                }
            }
            catch (Exception)
            { }
            finally
            {
                connection.Close();
            }
            return string.Join("\n", result);
        }
        public List<Course> GetCourses()
        {
            var courses = new List<Course>();
            try
            {
                connection.Open();
                command.CommandText = "SELECT name_of_courses, credits FROM Courses1";
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    courses.Add(new Course(
                        reader["name_of_courses"]?.ToString() ?? string.Empty,
                        reader["credits"]?.ToString() ?? string.Empty
                    ));
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return courses;
        }
        public Dictionary<string, int> GetAvailableCourses()
        {
            var courses = new Dictionary<string, int>();
            try
            {
                connection.Open();
                command.CommandText = "SELECT name_of_courses, credits FROM Courses1";
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    courses.Add(reader["name_of_courses"].ToString()!, reader.GetInt32(reader.GetOrdinal("credits")));
                }
            }
            catch (Exception)
            { }
            finally
            {
                connection.Close();
            }
            return courses;
        }
        public void DeleteCourse(int id)
        {
            try
            {
                connection.Open();
                command.CommandText = "DELETE FROM Courses1 WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new Exception($"Course with name {id} not found.");
                }
            }
            catch (Exception)
            { }
            finally
            {
                connection.Close();
            }
        }
        public void DeleteGrade(int id)
        {
            try
            {
                connection.Open();
                command.CommandText = "UPDATE Courses1 SET grade = NULL WHERE id = @id";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@id", id);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new Exception($"Course with id {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }
        public void AddCourse(AddCourse addCourse)
        {
            try
            {
                connection.Open();
                command.CommandText = "SELECT COUNT(*) FROM Courses1 WHERE name_of_courses = @name";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@name", addCourse.Name);
                int count = (int)command.ExecuteScalar();
                if (count > 0)
                {
                    command.CommandText = "UPDATE Courses1 SET credits = @credits WHERE name_of_courses = @name";
                }
                else
                {
                    command.CommandText = "INSERT INTO Courses1 (id, name_of_courses, credits, grade) VALUES (@id, @name, @credits, @grade)";
                    command.Parameters.AddWithValue("@id", addCourse.Id);
                }
                command.Parameters.AddWithValue("@credits", addCourse.Credits);
                command.Parameters.AddWithValue("@grade", addCourse.Grade);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }
        public void UpdateCourse(AddCourse updateCourse)
        {
            try
            {
                connection.Open();
                command.CommandText = "SELECT COUNT(*) FROM Courses1 WHERE id = @id";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@id", updateCourse.Id);
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    command.CommandText = "UPDATE Courses1 SET name_of_courses = @name, credits = @credits, grade = @grade WHERE id = @id";
                    command.Parameters.AddWithValue("@name", updateCourse.Name);
                    command.Parameters.AddWithValue("@credits", updateCourse.Credits);
                    command.Parameters.AddWithValue("@grade", updateCourse.Grade);
                    command.ExecuteNonQuery();
                }
                else
                {
                    Console.WriteLine("Course not found, unable to update.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
