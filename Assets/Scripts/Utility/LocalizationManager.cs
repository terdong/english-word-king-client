using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TeamGehem
{
    public class LocalizationManager
    {
        private static readonly string file_extention = "Local_*.csv";
        private static readonly string error_message = "not found message";
        private static readonly string key_file_name = "Local_Lang";
        private static LocalizationManager Instance_ = new LocalizationManager();

        private Dictionary<int, IList<string>> messages_dic_ = new Dictionary<int, IList<string>>();
        private IList<string> current_messages_;
        private int current_messages_count_;

        public string Directory_Path { set { directory_path_ = value; Instance_.Initialize_(); } get { return directory_path_; } }
        private string directory_path_ = "Assets/Resources";
        public static void SetLocale(TeamGehem.Local_Lang lang)
        {
            int index = (int)lang;
            Instance_.SetLocale_(ref index);
        }
        public static string GetMessage(TeamGehem.Local_Message local)
        {
            int index = (int)local;
            return Instance_.GetMessage_(ref index);
        }
        public static string GetMessage(TeamGehem.Local_System local)
        {
            int index = (int)local;
            return Instance_.GetMessage_(ref index);
        }
        public static void SetLocale(int local_index)
        {
            Instance_.SetLocale_(ref local_index);
        }
        public static string GetMessage(int index)
        {
            return Instance_.GetMessage_(ref index);
        }

        private void SetLocale_(ref int local_index)
        {
            if (messages_dic_.ContainsKey(local_index))
            {
                current_messages_ = messages_dic_[local_index];
                current_messages_count_ = current_messages_.Count;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        private string GetMessage_(ref int index)
        {
            if (index >= current_messages_count_)
            {
                return error_message;
            }
            return current_messages_[index];
        }

        private LocalizationManager()
        {
            Initialize_();
        }

        private void Initialize_()
        {
            List<string> line_list = new List<string>();

            DirectoryInfo directory_info = new DirectoryInfo(Directory_Path);

            FileInfo[] file_info_array = directory_info.GetFiles(string.Format("*{0}", file_extention));
            //File_info_array.OrderBy(f => f.Name); Should not linq
            SortFileInfo(ref file_info_array);

            int file_info_array_length = file_info_array.Length;

            messages_dic_.Clear();
            current_messages_ = null;

            bool is_not_init_dic = true;

            for (int file_index = 0; file_index < file_info_array_length; ++file_index)
            {
                FileInfo file_info = file_info_array[file_index];
                string file_name = file_info.Name.TrimEnd(file_extention.ToCharArray());

                if (file_name.Equals(key_file_name)) { continue; }

                using (TextReader tr = file_info.OpenText())
                {
                    string line;
                    while (!string.IsNullOrEmpty(line = tr.ReadLine()))
                    {
                        line_list.Add(line);
                    }
                }

                string[] key_parts = line_list[0].Split(',');
                int key_parts_length = key_parts.Length;
                int part_length = line_list.Count;

                if (is_not_init_dic)
                {
                    for (int i = 0; i < key_parts_length; ++i)
                    {
                        messages_dic_[i] = new List<string>();
                    }
                    is_not_init_dic = false;
                }

                for (int i = 0; i < part_length; ++i)
                {
                    var parts = line_list[i].Split(',');
                    for (int j = 1; j < key_parts_length; ++j)
                    {
                        IList<string> list = messages_dic_[j - 1];
                        list.Add(parts[j]);
                    }
                }
                line_list.Clear();
            }

            line_list = null;
            directory_info = null;
            file_info_array = null;

            if (file_info_array_length > 0)
            {
                int key_index = 0;
                SetLocale_(ref key_index);
            }
        }

        private void SortFileInfo(ref FileInfo[] fileEntries)
        {
            Array.Sort(fileEntries,
               delegate(FileInfo x, FileInfo y)
               {
                   return y.Name.CompareTo(x.Name);
               }
             );
        }
    }
}
