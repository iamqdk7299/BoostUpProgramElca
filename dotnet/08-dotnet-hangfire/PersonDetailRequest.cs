namespace HangfireExercise
{
    public class PersonDetailRequest
    {
        public PersonDetailRequest(long id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }

        public long Id { get; set; }
        public string FullName { get; set; }
    }
}