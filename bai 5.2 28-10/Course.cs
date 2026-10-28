namespace StudentRegistrationApp
{
    public class Course
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }

        public Course(string id, string name)
        {
            CourseId = id;
            CourseName = name;
        }
    }
}