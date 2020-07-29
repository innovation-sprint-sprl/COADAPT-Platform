using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {

	public interface IStudyRepository {

		Task<IEnumerable<Study>> StudiesBySupervisor(int supervisorId);
		Task<IEnumerable<Study>> StudiesByOrganization(int organizationId);
		Task<IEnumerable<Study>> GetAllStudiesAsync();
		Task<Study> GetStudyByIdAsync(int studyId);
		Task<Study> GetStudyOfOrganizationByShortnameAsync(string shortname, int organizationId);
		Task<IEnumerable<Study>> GetStudiesBySupervisorIdAsync(int supervisorId);
		Task<IEnumerable<Study>> GetStudiesByOrganizationIdAsync(int organizationId);
		Task<int> CountStudiesBySupervisorIdAsync(int supervisorId);
		Task<int> CountStudiesByOrganizationIdAsync(int organizationId);
		void CreateStudy(Study study);
		void UpdateStudy(Study dbStudy, Study study);
		void DeleteStudy(Study study);

	}

}
