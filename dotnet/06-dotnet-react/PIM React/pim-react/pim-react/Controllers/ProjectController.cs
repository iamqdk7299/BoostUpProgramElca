using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pim_react.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : ControllerBase
    {
        private static readonly List<Project> AllProjects = new() {
            new Project (){Id = 1, Number = 123, Customer = "CustomerA", Name = "Project A", Group = ".NET", Members = "AAA,BBB,CCC", Status  = ProjectStatusEnum.New},
            new Project (){Id = 2, Number = 456, Customer = "CustomerB", Name = "Project B", Group = ".Java", Members = "AAA,BBB,CCC", Status  = ProjectStatusEnum.New},
            new Project (){Id = 3, Number = 789, Customer = "CustomerC", Name = "Project C", Group = ".NET", Members = "AAA,BBB,CCC", Status  = ProjectStatusEnum.Planned},
            new Project (){Id = 4, Number = 102, Customer = "CustomerD", Name = "Project D", Group = ".Java", Members = "AAA,BBB,CCC", Status  = ProjectStatusEnum.Planned},
            new Project (){Id = 5, Number = 142, Customer = "CustomerE", Name = "Project E", Group = ".NET", Members = "AAA,BBB,CCC", Status  = ProjectStatusEnum.InProgress},
            new Project (){Id = 6, Number = 478, Customer = "CustomerF", Name = "Project F", Group = ".Java", Members = "AAA,BBB,CCC", Status  = ProjectStatusEnum.InProgress},
            new Project (){Id = 7, Number = 639, Customer = "CustomerK", Name = "Project K", Group = ".NET", Members = "AAA,BBB,CCC", Status  = ProjectStatusEnum.Finished},
            new Project (){Id = 8, Number = 785, Customer = "CustomerJ", Name = "Project J", Group = ".Java", Members = "AAA,BBB,CCC", Status  = ProjectStatusEnum.Finished},
        };       

        private readonly ILogger<ProjectController> _logger;

        public ProjectController(ILogger<ProjectController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Project> Get()
        {
           return AllProjects;
        }

        [HttpGet]
        [Route("/project/search")]
        public IEnumerable<Project> SearchProjects([FromQuery]int number, [FromQuery] string prjName, [FromQuery] string customerName)
        {
            return AllProjects.Where(prj =>
                (number == 0 || prj.Number == number) && 
                (string.IsNullOrEmpty(prjName) || prj.Name.Contains(prjName.Trim())) &&
                (string.IsNullOrEmpty(customerName) || prj.Customer.Contains(customerName.Trim())));
        }


        [HttpPost]
        [Route("/project/{prjId:int}")]
        public Project UpdateProject(int prjId, [FromBody] Project prj)
        {
            var updatedPrj = AllProjects.FirstOrDefault(p => p.Id == prjId);
            if (updatedPrj == null) {
                throw new KeyNotFoundException(string.Format("Not found project-number{0}", prj.Id));
            }
            updatedPrj.Number = updatedPrj.Number;
            updatedPrj.Name = updatedPrj.Name;
            updatedPrj.Customer = updatedPrj.Customer;
            updatedPrj.Group = updatedPrj.Group;
            updatedPrj.Members = updatedPrj.Members;
            updatedPrj.StartDate = updatedPrj.StartDate;
            updatedPrj.EndDate = updatedPrj.EndDate;
            updatedPrj.Status = updatedPrj.Status;
            return updatedPrj;
        }

        [HttpDelete]
        [Route("/project/{prjId:int}")]
        public void RemoveProject(int prjId)
        {
            var deletedPrj = AllProjects.FirstOrDefault(prj => prj.Id == prj.Id);
            if (deletedPrj == null)
            {
                throw new KeyNotFoundException(string.Format("Not found project-number{0}", prjId));
            }
            AllProjects.Remove(deletedPrj);           
        }

        [HttpPost]
        public void AddProject([FromBody] Project prj)
        {
            prj.Id = AllProjects.Max(p => p.Id) + 1;
            AllProjects.Add(prj);
        }
    }
}
