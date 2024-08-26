using GPACalculator.Models;
namespace GPACalculator.Services
{    
    public class GPACalculatorService
    {
        private readonly DbManager _dbManager;
        public GPACalculatorService()
        {
            _dbManager = new DbManager();
        }
        public double CalculateGPA()
        {
            return _dbManager.CalculateGPA(GetCourses());
        }
        public List<Course> GetCourses()
        {
            return _dbManager.GetCourses();
        }
        public Dictionary<string, int> GetAvailableCourses()
        {
            return _dbManager.GetAvailableCourses();
        }
        public void DeleteCourse(int id)
        {
            _dbManager.DeleteCourse(id);
        }
        public void UpdateCourse(AddCourse addCourse)
        {
            _dbManager.AddCourse(addCourse);
        }
    }
}
