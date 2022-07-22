using System;

namespace pim_react
{
    public class Project
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Customer { get; set; }
        public string Group { get; set; }
        public string Members { get; set; }
        public ProjectStatusEnum Status { get; set; }
    }
}
