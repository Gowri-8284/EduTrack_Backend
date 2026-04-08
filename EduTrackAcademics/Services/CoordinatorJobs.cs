using EduTrackAcademics.Service;

namespace EduTrackAcademics.Services

{

	public class CoordinatorJobs
	{

		private readonly CoordinatorDashboardService _service;


		public CoordinatorJobs(CoordinatorDashboardService service)

		{

			_service = service;

		}


		public void AutoAssignPendingStudentsJob()

		{

			_service.AutoAssignPendingStudents();

		}

	}


}