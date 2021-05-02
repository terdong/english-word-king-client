using System.Collections;
using SQLite4Unity3d;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    [NotNull, Unique]
    public string word_name { get; set; }
    [NotNull]
    public string word_mean { get; set; }
    public override string ToString()
    {
        return string.Format("[Word: Id={0}, Name={1}, Mean={2}]", id, word_name, word_mean);
    }
}

public class Word
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    [NotNull, Unique]
    public string word_name { get; set; }
    [NotNull]
    public string word_mean { get; set; }
    public override string ToString()
    {
        return string.Format("[Word: Id={0}, Name={1}, Mean={2}]", id, word_name, word_mean);
    }
}

public class WordInfo
{
    [PrimaryKey, Unique, NotNull]
    public int word_id { get; set; }
    public int level { get; set; }
    public int correct_answer { get; set; }
    public int wrong_answer { get; set; }
    public override string ToString()
    {
        return string.Format("[WordInfo: Id={0}, incorrect_answer={1}, wrong_answer={2}]", word_id, correct_answer, wrong_answer);
    }
}

