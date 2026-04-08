namespace EduTrackAcademics.DTO

{

	// Used for Add/Update (POST/PUT)
	public class ProgramDTO    {        
	public string ProgramName { get; set; }

	public string QualificationId { get; set; }

}

// Used for Fetching (GET)
public class ProgramResponseDTO    {        
public string ProgramId { get; set; }

public string ProgramName { get; set; }

public string QualificationId { get; set; }

    }

}