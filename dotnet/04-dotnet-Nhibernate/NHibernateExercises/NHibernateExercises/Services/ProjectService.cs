using NHibernateCore;
using NHibernateExercises.Entities;
using NHibernateExercises.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using NHibernateCore.Enver;

namespace NHibernateExercises.Services
{
    public class ProjectService : IProjectService
    {
        private const string Date_Format = "dd.MM.yyyy";
        private const string Date_Time_Format = "yyyy_MM_dd_HH_ss";
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;

        public ProjectService(IProjectRepository projectRepository, IUnitOfWorkProvider unitOfWorkProvider)
        {
            _projectRepository = projectRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
        }

        public void ProcessProject(int number)
        {
            try
            {
                Process(number);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error to process for number : " + number + ". Reason : " + ex.InnerException?.Message);
            }
        }

        private void Process(int number)
        {
            using var uow = _unitOfWorkProvider.Provide();
            InsertProject(number);
            ExportProjects(number);
            uow.Complete();
        }

        public List<ProjectEntity> GetAllProjectAudit()
        {
            using var uow = _unitOfWorkProvider.Provide();
            var result = uow.Session.Auditer().CreateQuery().ForRevisionsOfEntity(typeof(ProjectEntity), true, true)
                .GetResultList<ProjectEntity>();

            uow.Complete();
            return result.ToList();
        }

        private List<RevInfo> BuildListRevInfoPerEntry(IList revisions)
        {
            var resultList = new List<RevInfo>();
            foreach (var element in revisions)
            {
                var t = new RevInfo();
                var eP = element as IList<object>;
                t.UpdateData(eP);
                resultList.Add(t);
            }
            return resultList;
        }

        


        private void ExportProjects(int number)
        {
            WriteToFile(number, LoadProjectByNumbers(number));
        }

        private IList<ProjectEntity> LoadProjectByNumbers(int number)
        {
            using var uow = _unitOfWorkProvider.Provide();
            var projects = _projectRepository.LoadProjectByNumbers(new List<int>() { number });
            uow.Complete();
            return projects;
        }

        private static void WriteToFile(int number, IList<ProjectEntity> projects)
        {
            if (projects.Any())
            {
                var context = new StringBuilder();
                foreach (var project in projects)
                {
                    context.AppendLine($"{project.Id},{project.Number},{project.Name},{project.StartDate.ToString(Date_Format)},{project.EndDate?.ToString(Date_Format)}");
                }
                System.IO.File.WriteAllText(@$"{number}_{DateTime.Now.ToString(Date_Time_Format)}.txt", context.ToString());
            }
        }

        private void InsertProject(int number)
        {
            using var uow = _unitOfWorkProvider.Provide();
            _projectRepository.SaveOrUpdate(new Entities.ProjectEntity
            {
                Number = number,
                Name = "ABC",
                StartDate = DateTime.Now,
                RowVersion = 1
            });
            uow.Complete();
        }

        public void ProcessProjectInParallel(List<int> numbers)
        {
            // We use parallel process to improve the performance
            // please don't change anything
            Parallel.ForEach(
                        numbers,
                        new ParallelOptions { MaxDegreeOfParallelism = 2 },
                        number => ProcessProject(number));
        }

        public IList<int> GetAllProjectNumbers()
        {
            using var uow = _unitOfWorkProvider.Provide();
            var result = _projectRepository.GetAllProjectNumbers();
            uow.Complete();
            return result;
        }

        public void ImportProjects()
        {
            using var uow = _unitOfWorkProvider.Provide();
            var maxNumber = _projectRepository.GetMaxNumber();
            for (int i = 0; i < 200000; i++)
            {
                maxNumber++;
                var project = new Entities.ProjectEntity
                {
                    Number = maxNumber,
                    Name = "ABC",
                    StartDate = DateTime.Now,
                    RowVersion = 1
                };
                _projectRepository.Add(project);
            }
            uow.Complete();
        }
    }
}
