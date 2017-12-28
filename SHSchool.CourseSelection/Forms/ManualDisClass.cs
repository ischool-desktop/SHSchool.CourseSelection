﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using SHSchool.Data;
using FISCA.Data;
using FISCA.UDT;
using DevComponents.DotNetBar;
using System.Drawing.Drawing2D;
using DevComponents.DotNetBar.Rendering;


namespace SHSchool.CourseSelection.Forms
{
    public partial class ManualDisClass : BaseForm
    {
        // DIC
        Dictionary<string, string> subjectIDdic = new Dictionary<string, string>();
        Dictionary<string, string> studentIDdic = new Dictionary<string, string>();
        // List
        private SortedList<string, string> _CourseName = new SortedList<string, string>();
        private SortedList<string, Color> _CourseColor = new SortedList<string, Color>();
        // 紀錄選修課程分配到的學生
        Dictionary<string, List<string>> _CourseStudentIDdic = new Dictionary<string, List<string> >();

        public ManualDisClass()
        {
            InitializeComponent();

            // SchoolYear ComboBox
            schoolYearCbx.Text = SHSchoolInfo.DefaultSchoolYear;
            for (int i = 0;i < 3; i++)
            {
                schoolYearCbx.Items.Add(int.Parse(SHSchoolInfo.DefaultSchoolYear) + i );
            }

            // Semester ComboBox
            semesterCbx.Text = SHSchoolInfo.DefaultSemester;
            semesterCbx.Items.Add(1);
            semesterCbx.Items.Add(2);

            // Subject ComboBox
            AccessHelper accessSubject = new AccessHelper();
            List<UDT.Subject> s_list = accessSubject.Select<UDT.Subject>();
            foreach (UDT.Subject sc in s_list)
            {
                if (int.Parse(schoolYearCbx.Text) == sc.SchoolYear && int.Parse(semesterCbx.Text) == sc.Semester)
                {
                    subjectCbx.Items.Add(sc.SubjectName);
                    subjectIDdic.Add(sc.SubjectName,sc.UID);
                }
            }
        }

        private void subjectCbx_TextChanged(object sender, EventArgs e)
        {
            #region Course ButtonX
            // Course ButtonX
            this.flowLayoutPanel1.Controls.Clear();
            AccessHelper accessCourse = new AccessHelper();
            List<UDT.SubjectCourse> sc_list = accessCourse.Select<UDT.SubjectCourse>();

            int i = 0;

            Color[] colors = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.PowderBlue, Color.Orange, Color.Green, Color.Purple };

            foreach (UDT.SubjectCourse sc in sc_list)
            {
                if (subjectCbx.Text == sc.SubjectName && int.Parse(schoolYearCbx.Text) == sc.SchoolYear && int.Parse(semesterCbx.Text) == sc.Semester)
                {
                    ButtonX button = new ButtonX();
                    button.FocusCuesEnabled = false;
                    button.Style = eDotNetBarStyle.Office2007;
                    button.ColorTable = eButtonColor.Flat;
                    button.AutoSize = true;
                    button.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(15);
                    button.TextAlignment = eButtonTextAlignment.Left;
                    button.Size = new Size(110, 23);
                    button.Text = sc.CourseName;
                    button.Image = GetColorBallImage(colors[i++]);
                    button.Tag = "" + sc.CourseID;
                    button.Margin = new System.Windows.Forms.Padding(3);
                    button.Click += new EventHandler(Swap);
                    _CourseName.Add("" + sc.CourseID,sc.CourseName);
                    _CourseColor.Add("" + sc.CourseID,colors[i++]);
                    this.flowLayoutPanel1.Controls.Add(button);
                }
            }
            #endregion

            #region DataGridView 
            AccessHelper access = new AccessHelper();
            // 取得選課資料
            List<UDT.SSAttend> ssa_list = access.Select<UDT.SSAttend>();
            foreach (UDT.SSAttend ssa in ssa_list)
            {
                if (ssa.SubjectID == int.Parse("" + subjectIDdic[subjectCbx.Text]))
                {
                    studentIDdic.Add("" + ssa.StudentID,"" + ssa.SubjectID);
                }
            }
            // 取得學生資料
            List<SHStudentRecord> shs_list = SHStudent.SelectByIDs(studentIDdic.Keys.ToList());
            foreach (SHStudentRecord shs in shs_list)
            {
                DataGridViewRow datarow = new DataGridViewRow();
                datarow.CreateCells(dataGridViewX1);

                int index = 0;
                datarow.Cells[index++].Value = shs.StudentNumber;
                datarow.Cells[index++].Value = shs.Name;
                datarow.Cells[index++].Value = shs.Gender;
                datarow.Cells[index++].Value = shs.Class.Name;
                datarow.Cells[index++].Value = shs.SeatNo;

                string sql = string.Format(@"
                    SELECT ref_course_id
                    FROM $ischool.course_selection.ss_attend
                    WHERE 
                        ref_student_id = {0}
                        AND ref_course_id is not null
                ", shs.ID);
                QueryHelper qh = new QueryHelper();
                DataTable courseNameDt =  qh.Select(sql);
                foreach (DataRow dr in courseNameDt.Rows)
                {
                    SHCourseRecord sr = SHCourse.SelectByID("" + dr["ref_course_id"]);
                    if (sr != null)
                    {
                        datarow.Cells[index++].Value = "" + sr.Name;
                        datarow.Cells[index++].Value = "" + sr.Name;
                    }
                }

                datarow.Tag = shs.ID;
                dataGridViewX1.Rows.Add(datarow);
            }

            #endregion

        }


        private void Swap(object sender,EventArgs e)
        {
            ButtonX button = (ButtonX)sender;
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                // 紀錄學生手動分配到的課程ID
                //studentCourseIDdic.Add("" + row.Tag,"" + button.Tag);
                if (_CourseStudentIDdic.ContainsKey("" + button.Tag))
                {
                    _CourseStudentIDdic["" + button.Tag].Add("" + row.Tag);
                }
                else
                {
                    _CourseStudentIDdic.Add("" + button.Tag, new List<string>());
                    _CourseStudentIDdic["" + button.Tag].Add("" + row.Tag);
                }

                // 修改 DataGridView
                dataGridViewX1.Rows[row.Index].Cells[6].Value = _CourseName["" + button.Tag];

            }
            CountStudents();
        }

        private void CountStudents()
        { 
            foreach (Control btn in flowLayoutPanel1.Controls)
            {
                string courseID = "" + btn.Tag;
                if (!_CourseStudentIDdic.ContainsKey(courseID))
                {
                    btn.Text = _CourseName[courseID] + "(0人)";
                }
                else
                {
                    int totle = _CourseStudentIDdic[courseID].Count();
                    int b = 0, g = 0;

                    foreach (SHStudentRecord sr in SHStudent.SelectByIDs(_CourseStudentIDdic[courseID].ToArray()))
                    {
                        
                        if (sr.Gender == "男") b++;
                        if (sr.Gender == "女") g++;
                    }
                    btn.Text = _CourseName[courseID] + "(" + (b > 0 ? " " + b + "男" : "") + (g > 0 ? " " + g + "女" : "") + (totle - b - g > 0 ? " " + (totle - b - g) + "未知性別" : "") + " 共" + totle + "人" + " )";
                }
            }
        }

        public Image GetColorBallImage(Color color)
        {
            Bitmap bmp = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            int w = 14,
                    x = 1,
                    y = 1;
            Color[] myColors = { color, Color.White, color, color };
            float[] myPositions = { 0.0f, 0.05f, 0.6f, 1.0f };
            ColorBlend myBlend = new ColorBlend();
            myBlend.Colors = myColors;
            myBlend.Positions = myPositions;
            using (LinearGradientBrush brush = new LinearGradientBrush(new Point(x, y), new Point(w, w), Color.White, color))
            {
                brush.InterpolationColors = myBlend;
                brush.GammaCorrection = true;
                graphics.FillRectangle(brush, x, y, w, w);
            }
            graphics.DrawRectangle(new Pen(Color.Black), x, y, w, w);
            return bmp;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            //_CourseStudentIDdic
            Dictionary<string, string> studentCourseIDdic = new Dictionary<string, string>();
            foreach (var csi in _CourseStudentIDdic)
            {
                foreach (string si in csi.Value)
                {
                    studentCourseIDdic.Add(si,csi.Key);
                }
            }
            // 學生分班資訊Update SSAttend_UDT
            AccessHelper access = new AccessHelper();
            List<UDT.SSAttend> sa_list = access.Select<UDT.SSAttend>();

            foreach (UDT.SSAttend sa in sa_list)
            {
                foreach (var sc in studentCourseIDdic)
                {
                    if (sa.StudentID == int.Parse("" + sc.Key))
                    {
                        sa.CourseID = int.Parse("" + sc.Value);
                    }
                }
            }
            sa_list.SaveAll();

        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}