using System;
using System.Collections.Generic;
using System.Text;

namespace Ebor.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository() : base ()
        {
        }

        public IEnumerable<Course> GetCoursesWithAuthors(int pageIndex, int pageSize)
        {
            // Here you would use go to the database and get rows that match.
            // These are then returned as Course objects.
            return null;
        }

        public IEnumerable<Course> GetTopSellingCourses(int count)
        {
            // Here you would use go to the database and get rows that match.
            // These are then returned as Course objects.
            return null;
        }
    }
}
