using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using 의료IT공학과.데이터베이스;

namespace 학생정보입력프로그램
{
    /// <summary>
    /// 학생정보관리.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class 학생정보관리 : Window
    {
        static xLocalDB db = new xLocalDB("Provider=Microsoft.ACE.OLEDB.12.0; " +
                                       "Data Source=../../../DBFiles/SUGANG_DB.accdb;" +
                                       "Persist Security Info=False");

        string query학과 = "";     //학과 데이터 출력
        string query학생 = "";     //해당학과 학생 데이터 출력
        string query학번 = "";     //학생 데이터 출력
        string query상태 = "";     //상태 데이터 출력
        string query삭제 = "";

        string selected학과 = "";  //선택된 학과
        string selected학번 = "";  //선택된 학번

        string query학과입력 = "";
        string query상태입력 = "";

        string 학번 = "";
        string 이름 = "";
        string 비밀번호 = "";
        string 학과 = "";
        string 상태 = "";
        int 상태코드;
        string 주소 = "";
        string 이메일 = "";
        string 전화 = "";

        //----------------------------------------------
        public 학생정보관리()
        {
            InitializeComponent();
            InitializeSetting();    //초기 학과 콤보박스 데이터 생성
            All_text_clear();       //입력박스 초기과
        }
        //----------------------------------------------

        //----------------------------------------------
        private void InitializeSetting()
        {
            db.Open();
            string query학과 = "SELECT xDept_name FROM xDEPARTMENT GROUP BY xDept_name";
            string err_msg학과 = db.Query(query학과);

            if (err_msg학과 != null)
            {
                MessageBox.Show(query학과 + "\n\n" + err_msg학과);
            }

            InitializeTable학과();
            InitializeCombo상태();

            db.Close();

        }
        //----------------------------------------------

        //----------------------------------------------
        private void InitializeCombo상태()
        {
            query상태 = "SELECT xStatus_title FROM xSTUDENT_STATUS";
            string err_msg상태 = db.Query(query상태);

            if (err_msg상태 != null)
            {
                MessageBox.Show(query학과 + "\n\n" + query상태);
            }
            cbx_상태입력.Items.Clear();

            while (db.Read())
            {
                for (int i = 0; i < db.FieldCount; i++)
                {
                    cbx_상태입력.Items.Add(db.GetData(i));
                }
            }
        }
        //----------------------------------------------

        //----------------------------------------------
        private void InitializeTable학과()
        {
            cbx_학과선택.Items.Clear();
            while (db.Read())
            {
                for (int i = 0; i < db.FieldCount; i++)
                {
                    cbx_학과선택.Items.Add(db.GetData(i));
                    cbx_학과입력.Items.Add(db.GetData(i));
                }
            }
        }
        //----------------------------------------------

        //----------------------------------------------
        private void All_text_clear()
        {
            txtBox_학번.Text = "";
            txtBox_학번.IsReadOnly = true;
            txtBox_이름.Text = "";
            txtBox_비밀번호.Text = "";
            txtBox_비밀번호_확인.Password = "";
            txtBox_주소.Text = "";
            txtBox_이메일.Text = "";
            txtBox_전화.Text = "";

            psBox_비밀번호.Password = "";
            psBox_비밀번호.Visibility = Visibility.Hidden;

            cbx_학과입력.SelectedValue = "";
            cbx_상태입력.SelectedValue = "";
        }
        //----------------------------------------------

        //----------------------------------------------
        private void Cbx_학과선택_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx학과선택 = sender as ComboBox;
            if (cbx학과선택 == null) return;

            if (cbx학과선택.Items.Count == 0) return;

            All_text_clear(); // 초기화

            if (cbx_학과선택.SelectedIndex > -1)
            {
                selected학과 = cbx_학과선택.SelectedValue.ToString();

                dataGrid_정보.ItemsSource = "";
                dataGrid_정보.Items.Refresh();

                db.Open();

                query학생 = "SELECT xStatus_title, xHakbun, xName FROM xSTUDENTS, xDEPARTMENT, xSTUDENT_STATUS WHERE xDept = xDept_code AND xStatus = xStatus_code AND xDept_name = '" + selected학과 + "'";

                string err_msg학생 = db.Query(query학생);
                if (err_msg학생 != null)
                {
                    MessageBox.Show(query학생 + "\n\n" + err_msg학생, "SQL Error");
                }

                Initialize학생();    //학생테이블 생성

                db.Close();
            }

            btn_삭제.IsEnabled = true;
        }
        //----------------------------------------------

        //----------------------------------------------
        private void Initialize학생()
        {
            DataTable dataTable = new DataTable();
            if (db.HasRows)
            {
                dataTable.Columns.Add("상태", typeof(string));
                dataTable.Columns.Add("학번", typeof(string));
                dataTable.Columns.Add("이름", typeof(string));

                while (db.Read())
                {
                    DataRow row = dataTable.NewRow();
                    object[] rowArray = new object[db.FieldCount];

                    for (int i = 0; i < db.FieldCount; i++)
                    {
                        rowArray[i] = db.GetData(i);
                        row = dataTable.NewRow();
                        row.ItemArray = rowArray;
                    }
                    dataTable.Rows.Add(row);
                }
                dataGrid_정보.ItemsSource = dataTable.DefaultView;
                dataGrid_정보.DisplayMemberPath = "학번";
                dataGrid_정보.SelectedValuePath = "학번";
            }
        }
        //----------------------------------------------

        //----------------------------------------------
        private void DataGrid_정보_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            All_text_clear();
            if(psBox_비밀번호.Visibility == Visibility.Visible)
            {
                psBox_비밀번호.Visibility = Visibility.Hidden;
            }

            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid == null) return;

            if (dataGrid.Items.Count == 0) return;

            if (dataGrid_정보.SelectedValue == null) return;

            selected학번 = dataGrid_정보.SelectedValue.ToString();

            read학생정보();

            btn_삭제.IsEnabled = true;
        }
        //----------------------------------------------

        //----------------------------------------------
        private void read학생정보()
        {
            db.Open();

            query학번 = "SELECT * FROM xSTUDENTS, xDEPARTMENT, xSTUDENT_STATUS WHERE xDept = xDept_code AND xStatus = xStatus_code AND xDept_name = '" + selected학과 + "' AND xHakbun = '" + selected학번 + "'";
            string err_msg학번 = db.Query(query학번);

            if (err_msg학번 != null)
            {
                MessageBox.Show(query학번 + "\n\n" + err_msg학번, "SQL Error");
            }

            db.ExecuteReader(query학번);

            while (db.Read())
            {
                txtBox_학번.Text = db.GetData("xHakbun").ToString();
                txtBox_학번.IsReadOnly = true;
                txtBox_이름.Text = db.GetData("xName").ToString();
                txtBox_비밀번호.Text = db.GetData("xPassword").ToString();
                txtBox_주소.Text = db.GetData("xAddress").ToString();
                txtBox_이메일.Text = db.GetData("xEmail").ToString();
                txtBox_전화.Text = db.GetData("xPhone").ToString();
                cbx_상태입력.SelectedValue = db.GetData("xStatus_title").ToString();
                cbx_학과입력.SelectedValue = db.GetData("xDept_name").ToString();
            }

            db.Close();
        }
        //----------------------------------------------

        //----------------------------------------------
        private void Btn_추가_Click(object sender, RoutedEventArgs e)
        {
            All_text_clear();
            txtBox_학번.IsReadOnly = false;
            btn_삭제.IsEnabled = false;
            psBox_비밀번호.Visibility = Visibility.Visible;
        }
        //----------------------------------------------

        //----------------------------------------------
        private void Btn_확인_Click(object sender, RoutedEventArgs e)
        {
            selectInfo();

            if (학번 == "" || 이름 == "" || 비밀번호 == "" || 학과 == "" || 상태 == "" || 주소 == "" || 이메일 == "" || 전화 == "")
            {
                MessageBox.Show("항목을 입력해주제요");
                return;
            }

            if(psBox_비밀번호.IsVisible == true)
            {
                if(psBox_비밀번호.Password.ToString() != txtBox_비밀번호_확인.Password.ToString())
                {
                    MessageBox.Show("비밀번호가 일치하지 않습니다");
                    return;
                }
            }
            else
            {
                if (txtBox_비밀번호.Text.ToString() != txtBox_비밀번호_확인.Password.ToString())
                {
                    MessageBox.Show("비밀번호가 일치하지 않습니다");
                    return;
                }
            }

            btn_삭제.IsEnabled = true;

            if (txtBox_학번.IsReadOnly == true)
            {
                change학과to_code();      //학과타이틀을 학과코드로 변경
                change상태to_code();      //상태타이틀을 상태코드로 변경

                updateStudent(db, 이름, 비밀번호, 학과, 상태코드, 주소, 이메일, 전화);
            }
            else //추가 일 때
            {
                change학과to_code();      //학과타이틀을 학과코드로 변경
                change상태to_code();      //상태타이틀을 상태코드로 변경

                insertStudent(db, 학번, 이름, 비밀번호, 학과, 상태코드, 주소, 이메일, 전화);       //데이터입력
            }

        }
        //----------------------------------------------

        //----------------------------------------------
        private int updateStudent(xLocalDB db, string 이름, string 비밀번호, string 학과, int 상태코드, string 주소, string 이메일, string 전화)
        {

            if (IsValid_PEOPLE_NAME(이름) == false) return 0;

            if (IsValid_PASSWORD(비밀번호) == false) return 0;

            if (IsValid_HOUSE_ADDRESS(주소) == false) return 0;

            if (IsValid_EMAIL_ADDRESS(이메일) == false) return 0;

            if (IsValid_PHONE_NUMBER(전화) == false) return 0;

            //string str = string.Format("'{0}', '{1}', '{2}', '{3}', {4}, '{5}', '{6}'",
            //                            이름, 비밀번호, 학과, 상태코드, 주소, 이메일, 전화);

            string str = string.Format("xName = '{0}', xPassword = '{1}', xDept = '{2}', xStatus = {3}, xAddress = '{4}', xEmail = '{5}', xPhone = '{6}'",
                                        이름, 비밀번호, 학과, 상태코드, 주소, 이메일, 전화);

            return updateRow(db, "xSTUDENTS", str);
        }
        //----------------------------------------------

        //----------------------------------------------
        private int updateRow(xLocalDB db, string tableName, string str)
        {
            db.Open();

            string queryStr = string.Format("UPDATE " + tableName + " SET " + str + " WHERE xHakbun = '" + txtBox_학번.Text + "'");  //업데이트로 변경하기

            if (!DB_Query(db, queryStr)) return 0;

            MessageBox.Show("정상적으로 데이터가 변경되었습니다.");

            reloadInfo();
            All_text_clear();

            db.Close();
            return 1;  //OK
        }
        //----------------------------------------------

        //----------------------------------------------
        private void change상태to_code()
        {
            db.Open();

            query상태입력 = "SELECT xStatus_code FROM xSTUDENT_STATUS WHERE xStatus_title = '" + 상태 + "'";

            string err_msg상태입력 = db.Query(query상태입력);
            if (err_msg상태입력 != null)
            {
                MessageBox.Show(query상태입력 + "\n\n" + err_msg상태입력, "SQL Error");
            }

            db.ExecuteReader(query상태입력);

            while (db.Read())
            {
                상태 = db.GetData("xStatus_code").ToString();
            }

            db.Close();
            상태코드 = int.Parse(상태);   //문자열을 int형트로 바꿈
        }
        //----------------------------------------------

        //----------------------------------------------
        private void change학과to_code()
        {
            db.Open();

            query학과입력 = "SELECT xDept_code FROM xDEPARTMENT WHERE xDept_name = '" + 학과 + "'";

            string err_msg학과입력 = db.Query(query학과입력);
            if (err_msg학과입력 != null)
            {
                MessageBox.Show(query학과입력 + "\n\n" + err_msg학과입력, "SQL Error");
            }

            db.ExecuteReader(query학과입력);

            while (db.Read())
            {
                학과 = db.GetData("xDept_code").ToString();
            }

            db.Close();
        }
        //----------------------------------------------

        //----------------------------------------------
        private int insertStudent(iLocalDB db, string 학번, string 이름, string 비밀번호, string 학과, int 상태코드, string 주소, string 이메일, string 전화)
        {

            if (IsValid_IDENTIFIER(db, 학번) == false) return 0;

            if (IsNewHackbun(db, 학번) == false) return 0;

            if (IsValid_PEOPLE_NAME(이름) == false) return 0;

            if (IsValid_PASSWORD(비밀번호) == false) return 0;

            if (IsValid_HOUSE_ADDRESS(주소) == false) return 0;

            if (IsValid_EMAIL_ADDRESS(이메일) == false) return 0;

            if (IsValid_PHONE_NUMBER(전화) == false) return 0;

            string str = string.Format("'{0}', '{1}', '{2}', '{3}', {4}, '{5}', '{6}', '{7}'",
                                        학번, 이름, 비밀번호, 학과, 상태코드, 주소, 이메일, 전화);

            return insertRow(db, "xSTUDENTS", str);
        }
        //----------------------------------------------

        //----------------------------------------------
        private bool IsValid_PHONE_NUMBER(string 전화)
        {
            if (전화.Length > 20) return Error("전화 20문자를 초과할 수 없습니다");

            if (!stringContains_Oneof(전화, "-")) return Error("전화번호에는 -를 포함 되어야 합니다");

            if (전화.Substring(0, 2).ToString() != "01") return Error("맨 앞의 숫자 두개는 01이어야 합니다"); 

            return true;
        }
        //----------------------------------------------

        //----------------------------------------------
        private bool IsValid_EMAIL_ADDRESS(string 이메일)
        {
            if(이메일.Length > 30) return Error("이메일 30문자를 초과할 수 없습니다");

            if (!stringContains_Oneof(이메일, "@")) return Error("이메일에 @를 포함하여야 합니다");

            string[] splited = 이메일.Split('@');
            if (!stringContains_Oneof(splited[1], ".")) return Error("이메일에 @뒤에 적어도 한 개 이상의 .이 포함되어야 합니다");

            return true;
        }
        //----------------------------------------------        

        //----------------------------------------------
        private bool IsValid_HOUSE_ADDRESS(string 주소)
        {
            if (string.IsNullOrEmpty(주소)) return Error("주소 null이거나 비어있습니다");

            if (주소.Length > 50) return Error("주소는 50문자를 초과할 수 없습니다");

            string[] splited = 주소.Split(' ');
            if (!stringContains_Oneof(splited[0], "시도")) return Error("주소에 시/도를 포함하여야 합니다");

            return true;
        }
        //----------------------------------------------

        //----------------------------------------------
        private bool IsValid_PASSWORD(string 비밀번호)
        {
            if (string.IsNullOrEmpty(비밀번호)) return Error("비밀번호가 null이거나 비어있습니다");
            if (비밀번호.Length > 20 || 비밀번호.Length < 10) return Error("비밀번호 길이는 10문자이상 20문자이하로 입력해야 합니다");
            if (stringContains_Oneof(비밀번호, " \t\r\n")) return Error("비밀번호는 공백이 허용되지 않습니다");
            if (!stringContains_Oneof(비밀번호, "!@#$%^&*()")) return Error("비밀번호는 특수기호를 포함하여야 합니다");
            if (!stringContains_Oneof(비밀번호, "[A-Z]")) return Error("비밀번호는 영문대문자를 포함하여야 합니다");
            if (!stringContains_Oneof(비밀번호, "abcdefghijklmnopqrstuvwxyz")) return Error("비밀번호는 영문소문자를 포함하여야 합니다");
            if (!stringContains_Oneof(비밀번호, "0123456789")) return Error("비밀번호는 숫자를 포함하여야 합니다");
            return true;
        }
        //----------------------------------------------

        //----------------------------------------------
        private bool IsValid_PEOPLE_NAME(string 이름)
        {
            if (string.IsNullOrEmpty(이름)) return Error("사람 이름문자열이 null이거나 비어있습니다");
            if (이름.Length > 20) return Error("사람 이름은 20문자를 초과할 수 없습니다");
            if (stringContains_Oneof(이름, " \t\r\n")) return Error("사람이름은 공백이 허용되지 않습니다");
            return true;
        }
        //----------------------------------------------

        //----------------------------------------------
        private bool IsNewHackbun(iLocalDB db, string 학번)
        {
            string query = string.Format("SELECT xHakbun FROM xSTUDENTS WHERE xHakbun='{0}'", 학번);  // {0}에는 code 값이 들어감
            string res = db.Query(query);
            if (res == "")
            {
                MessageBox.Show(res);
                return false;
            }
            else if (db.HasRows)
            {
                return Error("같은 학번이 이미 존재합니다" + 학번);
            }
            else
                return true;
        }
        //----------------------------------------------

        //----------------------------------------------
        private bool IsValid_IDENTIFIER(iLocalDB db, string 학번)
        {
            if (string.IsNullOrEmpty(학번)) return Error("학번이 null이거나 빈문자열입니다.");
            if (학번.Length != 8) return Error("학번의 길이는 8문자이어야 합니다");
            if (IsNumericString(학번.Substring(0, 2)) == false) return Error("학번의 처음 두 문자는 년도를 나타내야 합니다.(예: 19)");
            if (학번.Substring(5, 3).Equals("000")) return Error("000은 올바른 학번의 일련번호가 아닙니다");
            if (IsNumericString(학번.Substring(5, 3)) == false) return Error("학번의 마지막 세 문자는 일련번호여야 합니다.(예: 001)");
            if (IsValidDeptCode(db, 학번.Substring(2, 3)) == false) return Error("학번의 3,4,5번째 문자에 일치하는 학과코드가 없습니다.");
            if (IsSameDeptCode(db, 학번.Substring(2, 3)) == false) return Error("학번과 학과가 일치하지 않습니다.");

            return true;
        }
        //----------------------------------------------

        //----------------------------------------------
        private bool IsValidDeptCode(iLocalDB db, string code)
        {
            string query = string.Format("SELECT * FROM xDEPARTMENT WHERE xDept_code='{0}'", code);  // {0}에는 code 값이 들어감

            string res = db.Query(query);
            if (res == "")
            {
                MessageBox.Show(res);
                return false;
            }
            if (db.HasRows)
            {
                return false;
            }
            else
                return true;
        }
        //----------------------------------------------

        //----------------------------------------------
        private bool IsSameDeptCode(iLocalDB db, string code)
        {
            db.Open();
            string query = string.Format("SELECT xDept_name FROM xDEPARTMENT WHERE xDept_code='{0}'", code);  // {0}에는 code 값이 들어감
            string err_msg = db.Query(query);
            string title = "";
            if (err_msg != null)
            {
                MessageBox.Show(query + "\n\n" + err_msg, "SQL Error");
                return false;
            }
            db.ExecuteReader(query);

            while (db.Read())
            {
                title = db.GetData("xDept_name").ToString();
            }
            if (title != cbx_학과입력.SelectedValue.ToString())  //입력 학과랑 학번이랑 틀림
            {
                return false;
            }
            else
                return true;
            
        }
        //----------------------------------------------

        //----------------------------------------------
        private static bool IsNumericString(string str)
        {
            for(int i = 0; i < str.Length; i++)
			{
                if ("0123456789".Contains(str.Substring(i, 1)) == false) return false;
            }
            return true;
        }
        //----------------------------------------------

        //----------------------------------------------
        private int insertRow(iLocalDB db, string tableName, string dataStr)
        {
            db.Open();

            string queryStr = string.Format("INSERT INTO {0} VALUES({1})", tableName, dataStr);

            if (!DB_Query(db, queryStr)) return 0;

            MessageBox.Show("정상적으로 데이터가 입력되었습니다.");

            psBox_비밀번호.Visibility = Visibility.Hidden;

            reloadInfo();
            All_text_clear();

            db.Close();
            return 1;  //OK
        }
        //----------------------------------------------

        //----------------------------------------------
        static bool DB_Query(iLocalDB db, string query)
        {
            string err_msg = db.Query(query);

            if (err_msg != null)
            {
                MessageBox.Show("Error\n" + err_msg + "\n" + query);
                return false;
            }
            return true;
        }
        //----------------------------------------------

        //----------------------------------------------
        private void Btn_닫기_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //----------------------------------------------

        //----------------------------------------------
        private void Btn_취소_Click(object sender, RoutedEventArgs e)
        {
            if (txtBox_학번.IsReadOnly == true)
            {
                read학생정보();
            }
            else
            {
                All_text_clear();
            }
        }
        //----------------------------------------------

        //----------------------------------------------
        private void selectInfo()
        {
            학번 = txtBox_학번.Text.ToString();
            이름 = txtBox_이름.Text.ToString();
            주소 = txtBox_주소.Text.ToString();
            이메일 = txtBox_이메일.Text.ToString();
            전화 = txtBox_전화.Text.ToString();
            if(psBox_비밀번호.IsVisible == true)
            {
                비밀번호 = psBox_비밀번호.Password;
            }
            else
            {
                비밀번호 = txtBox_비밀번호.Text.ToString();
            }
            if (cbx_학과입력.SelectedIndex > -1)
            {
                학과 = cbx_학과입력.SelectedValue.ToString();
            }
            if (cbx_상태입력.SelectedIndex > -1)
            {
                상태 = cbx_상태입력.SelectedValue.ToString();
            }
        }
        //----------------------------------------------

        //----------------------------------------------
        private void Btn_삭제_Click(object sender, RoutedEventArgs e)
        {
            if(txtBox_학번.Text == "")
            {
                MessageBox.Show("삭제하려는 데이터를 입력해주세요");
                return;
            }

            if (MessageBox.Show("해당데이터를 삭제하시겠습니까?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                db.Open();
                query삭제 = "DELETE FROM xSTUDENTS WHERE xHakbun = '" + selected학번 + "'";
                if(query삭제.ToString() == "")
                {
                    MessageBox.Show("삭제할 데이터가 없습니다");
                    return;
                }
                string err_msg삭제 = db.Query(query삭제);
                if (err_msg삭제 != null)
                {
                    MessageBox.Show(query삭제 + "\n\n" + err_msg삭제);
                }

                reloadInfo();
                All_text_clear();
                
                db.Close();
            }
            
        }
        //----------------------------------------------

        //----------------------------------------------
        private void reloadInfo()
        {
            dataGrid_정보.ItemsSource = "";
            dataGrid_정보.Items.Refresh();

            query학생 = "SELECT xStatus_title, xHakbun, xName FROM xSTUDENTS, xDEPARTMENT, xSTUDENT_STATUS WHERE xDept = xDept_code AND xStatus = xStatus_code AND xDept_name = '" + selected학과 + "'";

            string err_msg학생 = db.Query(query학생);
            if (err_msg학생 != null)
            {
                MessageBox.Show(query학생 + "\n\n" + err_msg학생, "SQL Error");
            }

            Initialize학생();    //학생테이블 생성
        }
        //----------------------------------------------

        //----------------------------------------------
        static bool stringContains_Oneof(string str, string oneof)
        {
            for (int i = 0; i < oneof.Length; i++)
            {
                if (str.Contains(oneof.Substring(i, 1))) return true;
            }

            return false;
        }
        //----------------------------------------------

        //----------------------------------------------
        static bool Error(string msg)
        {
            MessageBox.Show("*** Error: " + msg + " ***");
            return false;
        }
        //----------------------------------------------
    }
}