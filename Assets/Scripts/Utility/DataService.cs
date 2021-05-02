using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System;

public class DataService
{

    private SQLiteConnection _connection;
    private Type word_class_type_;
    private Type word_info_class_type_;
    public DataService(string DatabaseName)
    {

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
			var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
			// then save to Application.persistentDataPath
			File.Copy(loadDb, filepath);
#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);

        InitClassType();
    }

    //public IEnumerable<Word> GetWordTalbe()
    //{
    //    return _connection.Table<Word>();
    //}

    public IEnumerable<Word> GetWordTalbe()
    {
        return _connection.Table<Word>();
    }

    public Word GetWord(string word_name)
    {
        return _connection.Table<Word>().Where(x => x.word_name.Equals(word_name)).FirstOrDefault();
    }

    public Word CreateWord(string a_word_name, string a_word_mean)
    {
        var w = new Word
        {
            word_name = a_word_name,
            word_mean = a_word_mean
        };
        try
        {
            _connection.Insert(w, word_class_type_);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
        }

        CreateWordInfo(w.id);
        return w;
    }

    public int UpdateWord(Word word)
    {
        return _connection.Update(word, word_class_type_);
    }

    public WordInfo CreateWordInfo(int a_word_id)
    {
        var w_i = new WordInfo
        {
            word_id = a_word_id,
            level = 1
        };
        try
        {
            _connection.Insert(w_i, word_info_class_type_);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
        }
        return w_i;
    }

    public int UpdateWordInfo(WordInfo word_info)
    {
        return _connection.Update(word_info, word_info_class_type_);
    }

    public WordInfo GetWordInfo(int a_word_id)
    {
        return _connection.Table<WordInfo>().Where(x => x.word_id.Equals(a_word_id)).FirstOrDefault();
    }

    public void DropTables()
    {
        _connection.DropTable<Word>();
        _connection.CreateTable<Word>();
        _connection.DropTable<WordInfo>();
        _connection.CreateTable<WordInfo>();
    }

    private void InitClassType()
    {
        word_class_type_ = typeof(Word);
        word_info_class_type_ = typeof(WordInfo);
    }

/*
    public void CreateDB()
    {
        _connection.DropTable<Person>();
        _connection.CreateTable<Person>();

        _connection.InsertAll(new[]{
			new Person{
				Id = 1,
				Name = "Tom",
				Surname = "Perez",
				Age = 56
			},
			new Person{
				Id = 2,
				Name = "Fred",
				Surname = "Arthurson",
				Age = 16
			},
			new Person{
				Id = 3,
				Name = "John",
				Surname = "Doe",
				Age = 25
			},
			new Person{
				Id = 4,
				Name = "Roberto",
				Surname = "Huertas",
				Age = 37
			}
		});
    }

    public IEnumerable<Person> GetPersons()
    {
        return _connection.Table<Person>();
    }

    public IEnumerable<Person> GetPersonsNamedRoberto()
    {
        return _connection.Table<Person>().Where(x => x.Name == "Roberto");
    }

    public Person GetJohnny()
    {
        return _connection.Table<Person>().Where(x => x.Name == "Johnny").FirstOrDefault();
    }

    public Person CreatePerson()
    {
        var p = new Person
        {
            Name = "Johnny",
            Surname = "Mnemonic",
            Age = 21
        };
        _connection.Insert(p);
        return p;
    }
*/
}