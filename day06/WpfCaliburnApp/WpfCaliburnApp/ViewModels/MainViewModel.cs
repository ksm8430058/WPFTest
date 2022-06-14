using Caliburn.Micro;
using System.Data.SqlClient;
using WpfCaliburnApp.Models;

namespace WpfCaliburnApp.ViewModels
{
    public class MainViewModel : Screen
    {
        // private EmployeesModel employee;   //비쥬얼 스튜디어가 제안하는 이름 사용
        private BindableCollection<EmployeesModel> listEmployees; //list를 사용하지말고 BindableCollection
        public BindableCollection<EmployeesModel> ListEmployees
        {
            get { return listEmployees; }
            set
            {
                listEmployees = value;
                NotifyOfPropertyChange(() => ListEmployees);    //값의 변화가 있다면 무조건
            }
        }
        string connString = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";

        public MainViewModel()
        {
            GetEmployees();
        }

        public void GetEmployees()
        {
            using (SqlConnection conn = new SqlConnection(connString))  // 알아서 Close해줌
            {
                conn.Open();
                string strQuery = "SELECT * FROM TblEmployees";
                SqlCommand cmd = new SqlCommand(strQuery, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                ListEmployees = new BindableCollection<EmployeesModel>();

                while (reader.Read())
                {
                    var temp = new EmployeesModel
                    {
                        Id = (int)reader["Id"],
                        EmpName = reader["EmpName"].ToString(),
                        Salary = (decimal)reader["Salary"],
                        DeptName = reader["DeptName"].ToString(),
                        Destination = reader["Destination"].ToString()
                    };
                    ListEmployees.Add(temp);
                }
            }
        }
        int id;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyOfPropertyChange(() => Id);
                NotifyOfPropertyChange(() => CanDelEmployee);
            }
        }

        string empName;
        public string EmpName
        {
            get { return empName; }
            set
            {
                empName = value;
                NotifyOfPropertyChange(() => EmpName);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        decimal salary;

        public decimal Salary
        {
            get { return salary; }
            set
            {
                salary = value;
                NotifyOfPropertyChange(() => Salary);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        string deptName;
        public string DeptName
        {
            get { return deptName; }
            set
            {
                deptName = value;
                NotifyOfPropertyChange(() => DeptName);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        string destination;
        public string Destination
        {
            get { return destination; }
            set
            {
                destination = value;
                NotifyOfPropertyChange(() => Destination);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        private EmployeesModel selectedEmployee;
        public EmployeesModel SelectedEmployee
        {
            get { return selectedEmployee; }
            set
            {
                selectedEmployee = value;
                if (value != null)
                {
                    Id = value.Id;
                    EmpName = value.EmpName;
                    Salary = value.Salary;
                    DeptName = value.DeptName;
                    Destination = value.Destination;
                }
                NotifyOfPropertyChange(() => SelectedEmployee);
            }
        }
        public void NewEmployee()
        {
            Id = 0;
            EmpName = string.Empty;
            Salary = 0;
            DeptName = Destination = string.Empty;
        }

        //버튼 활성 ,비활성화 속성
        public bool CanSaveEmployee
        {
            get
            {
                return !string.IsNullOrEmpty(EmpName) &&
                       !string.IsNullOrEmpty(DeptName) &&
                       !string.IsNullOrEmpty(Destination) &&
                       Salary != 0;
            }
        }

        //버튼이벤트
        public void SaveEmployee()
        {
            //TODO
            using (SqlConnection conn = new SqlConnection(connString))  // 알아서 Close해줌
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                if (id == 0) //insert
                {
                    cmd.CommandText = @"INSERT INTO TblEmployees
                                                  (EmpName
                                                   ,Salary
                                                   ,DeptName
                                                   ,Destination)
                                             VALUES
                                                   (@EmpName
                                                   ,@Salary
                                                   ,@DeptName
                                                   ,@Destination)";
                }
                else //update
                    cmd.CommandText = @"UPDATE TblEmployees
                                           SET EmpName = @EmpName
                                              ,Salary = @Salary
                                              ,DeptName = @DeptName
                                              ,Destination = @Destination
                                         WHERE Id = @Id";

                SqlParameter parmEmpName = new SqlParameter("@EmpName", EmpName);
                SqlParameter parSalary = new SqlParameter("@Salary", Salary);
                SqlParameter parmDeptName = new SqlParameter("@DeptName", DeptName);
                SqlParameter parmDestination = new SqlParameter("@Destination", Destination);

                cmd.Parameters.Add(parmEmpName);
                cmd.Parameters.Add(parSalary);
                cmd.Parameters.Add(parmDeptName);
                cmd.Parameters.Add(parmDestination);

                if (Id != 0) //update 일때는 id값도 파라미터로 추가 해야한다.
                {
                    SqlParameter parmId = new SqlParameter("@Id", Id);
                    cmd.Parameters.Add(parmId);
                }

                cmd.ExecuteNonQuery();
            } // end of using (SQLCONNENTION...)
            //오른쪽의 입력창 초기화
            NewEmployee();
            //왼쪽의 데이터 다시 조회
            GetEmployees();
        }

        public bool CanDelEmployee
        {
            get { return (Id != 0); }
            
        }
        public void DelEmployee()
        {
            using (SqlConnection conn = new SqlConnection(connString))  // 알아서 Close해줌
            {
                conn.Open();
                string strQuery = "DELETE FROM TblEmployees WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(strQuery, conn);
                SqlParameter parmId = new SqlParameter("@Id", Id);
                cmd.Parameters.Add(parmId);
                cmd.ExecuteNonQuery();
            }
            NewEmployee();  //입력창초기화
            GetEmployees(); // 더이터 그리드 재호출
        }
    }
}
