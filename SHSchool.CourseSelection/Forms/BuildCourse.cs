﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data;
using FISCA.Data;
using SHSchool.Data;
using FISCA.UDT;

namespace SHSchool.CourseSelection.Forms
{
    public partial class BuildCourse : BaseForm
    {
        int schoolYear, semester;
        public BuildCourse(DataGridView dgv,int sy,int s,string type)
        {
            InitializeComponent();

            schoolYear = sy;
            semester = s;

            // Init Lb
            schoolYearLb.Text = "" + sy;
            semesterLb.Text = "" + s;

            #region Init DGV
            {
                foreach (DataGridViewRow dr in dgv.Rows)
                {
                    // 設定開班數
                    int count = int.Parse("" + dr.Cells["buildCourseCount"].Value);
                    // 已開班數
                    int _count = int.Parse("" + dr.Cells["courseCount"].Value);
                    
                    // 已開班數 = 設定開班數 : 修改
                    if (_count == count)
                    {
                        int sbID = int.Parse("" + dr.Tag);
                        QueryHelper qh = new QueryHelper();
                        string sql = string.Format(@"
                            SELECT *
                            FROM
                                $ischool.course_selection.subjectcourse
                            WHERE 
                                subject_id = {0}
                        ",sbID);
                        DataTable subjectCourseUDT = qh.Select(sql);
                        foreach (DataRow scr in subjectCourseUDT.Rows)
                        {
                            DataGridViewRow drX1 = new DataGridViewRow();
                            drX1.CreateCells(dataGridViewX1);
                            int index = 0;
                            drX1.Cells[index++].Value = "修改";
                            drX1.Cells[index++].Value = scr["course_name"];
                            drX1.Cells[index++].Value = scr["course_type"];
                            drX1.Cells[index++].Value = scr["subject_name"];
                            drX1.Cells[index++].Value = scr["level"];
                            drX1.Cells[index++].Value = "班別";
                            drX1.Cells[index++].Value = scr["credit"];
                            drX1.Tag = scr["uid"];

                            dataGridViewX1.Rows.Add(drX1);
                        }
                    }

                    // 已開班數 > 設定開班數 : 刪除
                    if (_count > count)
                    {
                        int sbID = int.Parse("" + dr.Tag);
                        QueryHelper qh = new QueryHelper();
                        string sql = string.Format(@"
                            SELECT *
                            FROM
                                $ischool.course_selection.subjectcourse
                            WHERE 
                                subject_id = {0}
                        ", sbID);
                        DataTable subjectCourseUDT = qh.Select(sql);
                        int n = 0;
                        foreach (DataRow scr in subjectCourseUDT.Rows)
                        {
                            DataGridViewRow drX1 = new DataGridViewRow();
                            drX1.CreateCells(dataGridViewX1);
                            n++;
                            if (n > count)
                            {
                                drX1.Cells[0].Value = "刪除";
                                drX1.DefaultCellStyle.BackColor = Color.Red;
                            }
                            else
                            {
                                drX1.Cells[0].Value = "修改";
                            }
                            int index = 1;
                            drX1.Cells[index++].Value = scr["course_name"];
                            drX1.Cells[index++].Value = scr["course_type"];
                            drX1.Cells[index++].Value = scr["subject_name"];
                            drX1.Cells[index++].Value = scr["level"];
                            drX1.Cells[index++].Value = "班別";
                            drX1.Cells[index++].Value = scr["credit"];
                            drX1.Tag = scr["uid"];

                            dataGridViewX1.Rows.Add(drX1);
                        }
                    }

                    // 已開班數 < 設定開班數 : 新增
                    if (_count < count)
                    {
                        int sbID = int.Parse("" + dr.Tag);
                        QueryHelper qh = new QueryHelper();
                        string sql = string.Format(@"
                            SELECT *
                            FROM
                                $ischool.course_selection.subjectcourse
                            WHERE 
                                subject_id = {0}
                        ", sbID);
                        DataTable subjectCourseUDT = qh.Select(sql);
                        int n = 0;
                        foreach (DataRow scr in subjectCourseUDT.Rows)
                        {
                            DataGridViewRow drX1 = new DataGridViewRow();
                            drX1.CreateCells(dataGridViewX1);
                            n++;
                            int index = 0;
                            drX1.Cells[index++].Value = "修改";
                            drX1.Cells[index++].Value = scr["course_name"];
                            drX1.Cells[index++].Value = scr["course_type"];
                            drX1.Cells[index++].Value = scr["subject_name"];
                            drX1.Cells[index++].Value = scr["level"];
                            drX1.Cells[index++].Value = "班別";
                            drX1.Cells[index++].Value = scr["credit"];
                            drX1.Tag = scr["uid"];

                            dataGridViewX1.Rows.Add(drX1);
                        }
                        for (int i = 1;i <= count - n; i++)
                        {
                            InitDataGridView("新增", i + n , type, dr);
                        }
                    }

                }
            }
            #endregion
        }

        public void InitDataGridView(string _dataType,int i,string type,DataGridViewRow dr)
        {
            #region Switch 級別、班別
            string[] mark = new string[10];
            string level = "";
            switch (i)
            {
                case 1:
                    mark[i] = "A";
                    break;
                case 2:
                    mark[i] = "B";
                    break;
                case 3:
                    mark[i] = "C";
                    break;
                case 4:
                    mark[i] = "D";
                    break;
                case 5:
                    mark[i] = "E";
                    break;
                case 6:
                    mark[i] = "F";
                    break;
                case 7:
                    mark[i] = "G";
                    break;
                case 8:
                    mark[i] = "H";
                    break;
                case 9:
                    mark[i] = "I";
                    break;
                case 10:
                    mark[i] = "J";
                    break;
                default:
                    MessageBox.Show("已超出開班數上限");
                    break;
            }

            switch (int.Parse("" + dr.Cells["level"].Value))
            {
                case 1:
                    level = "I";
                    break;
                case 2:
                    level = "II";
                    break;
                case 3:
                    level = "III";
                    break;
            }
            #endregion

            DataGridViewRow drX1 = new DataGridViewRow();
            drX1.CreateCells(dataGridViewX1);
            int index = 0;
            drX1.Cells[index++].Value = _dataType;
            drX1.Cells[index++].Value = type +" "+ dr.Cells["subjectName"].Value + " "+level +" "+ mark[i];
            drX1.Cells[index++].Value = type;
            drX1.Cells[index++].Value = dr.Cells["subjectName"].Value;
            drX1.Cells[index++].Value = dr.Cells["level"].Value;
            drX1.Cells[index++].Value = mark[i];
            drX1.Cells[index++].Value = dr.Cells["credit"].Value;
            if (_dataType == "刪除")
            {
                drX1.DefaultCellStyle.BackColor = Color.Red;
            }
            // 這邊的TAG 是 subjectID
            drX1.Tag = dr.Tag;
            dataGridViewX1.Rows.Add(drX1);
        }
        
        // 開課
        private void buildCourseBtn_Click(object sender, EventArgs e)
        {
            AccessHelper access = new AccessHelper();
            List<UDT.SubjectCourse> subCourseList = access.Select<UDT.SubjectCourse>();

            foreach (DataGridViewRow dr in dataGridViewX1.Rows)
            {
                
                if ("" + dr.Cells["dataType"].Value == "新增")
                {
                    //-- Course Table 新增課程資訊
                    //-- SubjectCourse UDT 同步 courseID
                    SHCourseRecord scr = new SHCourseRecord();
                    scr.Subject = "" + dr.Cells["subjectName"].Value;
                    scr.Level = int.Parse("" + dr.Cells["level"].Value);
                    scr.Name = "" + dr.Cells["courseName"].Value;
                    scr.Credit = int.Parse("" + dr.Cells["credit"].Value);
                    scr.SchoolYear = int.Parse(schoolYearLb.Text);
                    scr.Semester = int.Parse(semesterLb.Text);

                    string courseID = SHCourse.Insert(scr);
                    
                    // -- SubjectCourse UDT 新增 科目課程資訊
                    UDT.SubjectCourse sb = new UDT.SubjectCourse();
                    sb.SubjectID = int.Parse("" + dr.Tag);
                    sb.CourseID = int.Parse("" + courseID);
                    sb.SubjectName = "" + dr.Cells["subjectName"].Value;
                    sb.CourseName = "" + dr.Cells["courseName"].Value;
                    sb.Course_type = "" + dr.Cells["courseType"].Value;
                    sb.Level = int.Parse("" + dr.Cells["level"].Value);
                    sb.Credit = int.Parse("" + dr.Cells["credit"].Value);
                    sb.SchoolYear = int.Parse(schoolYearLb.Text);
                    sb.Semester = int.Parse(semesterLb.Text);

                    sb.Save();

                }
                if ("" + dr.Cells["dataType"].Value == "修改")
                {
                    UpdateHelper uph = new UpdateHelper();
                    string updateSql = string.Format(@"
                        UPDATE $ischool.course_selection.subjectcourse 
                        SET course_name = '{0}'
                        WHERE uid = {1}
                    ","" + dr.Cells["courseName"].Value, int.Parse("" + dr.Tag));
                    uph.Execute(updateSql);
                }
                if ("" + dr.Cells["dataType"].Value == "刪除")
                {
                    UpdateHelper uph = new UpdateHelper();

                    // 取得要刪除的課程ID
                    QueryHelper qh = new QueryHelper();
                    string selDeleteCourseID = string.Format(@"
                        SELECT course_id
                        From $ischool.course_selection.subjectcourse 
                        WHERE uid = {0}
                    ", int.Parse("" + dr.Tag));
                    DataTable dt = qh.Select(selDeleteCourseID);

                    // Course Table 刪除課程資訊
                    // 刪除課程資訊
                    foreach (DataRow datarow in dt.Rows)
                    {
                        int course_id = int.Parse("" + datarow["course_id"]);
                        string deleteSql = string.Format(@"
                            DELETE FROM course
                            WHERE id = {0}
                        ",course_id);
                        uph.Execute(deleteSql);
                    }

                    // SubjectCourse UDT 刪除科目課程資訊
                    string updateSql = string.Format(@"
                        DELETE FROM $ischool.course_selection.subjectcourse 
                        WHERE uid = {0}
                    ", int.Parse("" + dr.Tag));
                    uph.Execute(updateSql);
                }
            }
            
            MessageBox.Show( "課程建立成功!");
            this.Close();
        }
    }
}