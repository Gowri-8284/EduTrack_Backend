namespace EduTrackAcademics.DTO
{
	public class ModuleWithContentDto
	{
		public string ModuleID { get; set; }
		public string ModuleName { get; set; }
		public int SequenceOrder { get; set; }

		public List<ModuleContentDto> Contents { get; set; }
	}

	public class ModuleContentDto
	{
		public string ContentID { get; set; }
		public string Title { get; set; }
		public string ContentType { get; set; }
		public string ContentURI { get; set; }
		public TimeSpan? Duration { get; set; }
	}

}
