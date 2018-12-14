using System;
using System.Data;
using System.Text;

using Microsoft.Office.Interop;

namespace Advertisement.Web
{
	/// <summary>
	/// ����OWC11������ͳ��ͼ�ķ�װ�ࡣ
	/// ����ƽ 2005-8-31
	/// </summary>
	public class OWCChart11
	{

		#region ����
		private string _phaysicalimagepath;
		private string _title;
		private string _seriesname;
		private int _picwidth;
		private int _pichight;
		private DataTable _datasource;
		private string strCategory;
		private string strValue;

		public string PhaysicalImagePath
		{
			set{_phaysicalimagepath=value;}
			get{return _phaysicalimagepath;}
		}
		public string Title
		{
			set{_title=value;}
			get{return _title;}
		}
		public string SeriesName
		{
			set{_seriesname=value;}
			get{return _seriesname;}
		}

		public int PicWidth
		{
			set{_picwidth=value;}
			get{return _picwidth;}
		}

		public int PicHight
		{
			set{_pichight=value;}
			get{return _pichight;}
		}
		public DataTable DataSource
		{
			set
			{
				_datasource=value;
				strCategory=GetColumnsStr(_datasource);
				strValue=GetValueStr(_datasource);
			}
			get{return _datasource;}
		}

		private string GetColumnsStr(DataTable dt)
		{
			StringBuilder strList=new StringBuilder();
			foreach(DataRow r in dt.Rows)
			{
				strList.Append(r[0].ToString()+'\t');
			}
			return strList.ToString();
		}
		private string GetValueStr(DataTable dt)
		{
			StringBuilder strList=new StringBuilder();
			foreach(DataRow r in dt.Rows)
			{
				strList.Append(r[1].ToString()+'\t');
			}
			return strList.ToString();
		}

		#endregion


		public OWCChart11()
		{
		}
		public OWCChart11(string PhaysicalImagePath,string Title,string SeriesName)
		{
			_phaysicalimagepath=PhaysicalImagePath;
			_title=Title;
			_seriesname=SeriesName;		
		}

	
		/// <summary>
		/// ����ͼ
		/// </summary>
		/// <returns></returns>
		public string CreateColumn()
		{	
			Microsoft.Office.Interop.Owc11.ChartSpace objCSpace = new Microsoft.Office.Interop.Owc11.ChartSpaceClass();//����ChartSpace����������ͼ��			
			Microsoft.Office.Interop.Owc11.ChChart objChart  = objCSpace.Charts.Add(0);//��ChartSpace���������ͼ��Add��������chart����
											
			//ָ��ͼ������͡�������OWC.ChartChartTypeEnumö��ֵ�õ�//Microsoft.Office.Interop.OWC.ChartChartTypeEnum
			objChart.Type=Microsoft.Office.Interop.Owc11.ChartChartTypeEnum.chChartTypeColumnClustered;
			
			//ָ��ͼ���Ƿ���Ҫͼ��
			objChart.HasLegend = true;
			
			//����
			objChart.HasTitle = true;
			objChart.Title.Caption= _title;
//			objChart.Title.Font.Bold=true;
//			objChart.Title.Font.Color="blue";
								

			#region ��ʽ����		

//			//��ת
//			objChart.Rotation  = 360;//��ʾָ����άͼ�����ת�Ƕ�
//			objChart.Inclination = 10;//��ʾָ����άͼ�����ͼб�ʡ���Ч��ΧΪ -90 �� 90

			//������ɫ
//			objChart.PlotArea.Interior.Color = "red";

			//������ɫ
//			objChart.PlotArea.Floor.Interior.Color = "green";
// 
//			objChart.Overlap = 50;//��������б�־֮����ص���

			#endregion
			
			//x,y���ͼʾ˵��
			objChart.Axes[0].HasTitle = true;
			objChart.Axes[0].Title.Caption = "X �� ���";
			objChart.Axes[1].HasTitle = true;
			objChart.Axes[1].Title.Caption = "Y �� ����";
			

			//���һ��series
			Microsoft.Office.Interop.Owc11.ChSeries ThisChSeries = objChart.SeriesCollection.Add(0);


			//����series������
           ThisChSeries.SetData(Microsoft.Office.Interop.Owc11.ChartDimensionsEnum.chDimSeriesNames,
			   Microsoft.Office.Interop.Owc11.ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),SeriesName);
           //��������
           ThisChSeries.SetData(Microsoft.Office.Interop.Owc11.ChartDimensionsEnum.chDimCategories,
			   Microsoft.Office.Interop.Owc11.ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),strCategory);
           //����ֵ
           ThisChSeries.SetData(Microsoft.Office.Interop.Owc11.ChartDimensionsEnum.chDimValues,
			   Microsoft.Office.Interop.Owc11.ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),strValue);

			Microsoft.Office.Interop.Owc11.ChDataLabels dl=objChart.SeriesCollection[0].DataLabelsCollection.Add();			
			dl.HasValue=true;
//			dl.Position=Microsoft.Office.Interop.Owc11.ChartDataLabelPositionEnum.chLabelPositionOutsideEnd;

						
			string filename=DateTime.Now.ToString("yyyyMMddHHmmssff")+".gif";
			string strAbsolutePath = _phaysicalimagepath + "\\"+filename;
			objCSpace.ExportPicture(strAbsolutePath, "GIF", _picwidth, _pichight);//�����GIF�ļ�.

			return filename;
			
		}


        /// <summary>
        /// ��ͼ
        /// </summary>
        /// <returns></returns>
		public string CreatePie()
		{
			Microsoft.Office.Interop.Owc11.ChartSpace objCSpace = new Microsoft.Office.Interop.Owc11.ChartSpaceClass();//����ChartSpace����������ͼ��			
			Microsoft.Office.Interop.Owc11.ChChart objChart  = objCSpace.Charts.Add(0);//��ChartSpace���������ͼ��Add��������chart����
							
						
			//ָ��ͼ�������
			objChart.Type=Microsoft.Office.Interop.Owc11.ChartChartTypeEnum.chChartTypePie;
			
			//ָ��ͼ���Ƿ���Ҫͼ��
			objChart.HasLegend = true;
			
			//����
			objChart.HasTitle = true;
			objChart.Title.Caption= _title;
					
									
			//���һ��series
			Microsoft.Office.Interop.Owc11.ChSeries ThisChSeries = objChart.SeriesCollection.Add(0);

			//����series������
			ThisChSeries.SetData(Microsoft.Office.Interop.Owc11.ChartDimensionsEnum.chDimSeriesNames,
				Microsoft.Office.Interop.Owc11.ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),SeriesName);
			//��������
			ThisChSeries.SetData(Microsoft.Office.Interop.Owc11.ChartDimensionsEnum.chDimCategories,
				Microsoft.Office.Interop.Owc11.ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),strCategory);
			//����ֵ
			ThisChSeries.SetData(Microsoft.Office.Interop.Owc11.ChartDimensionsEnum.chDimValues,
				Microsoft.Office.Interop.Owc11.ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),strValue);
						

			//��ʾϵ�л��������ϵĵ������ݱ�־
			Microsoft.Office.Interop.Owc11.ChDataLabels dl=objChart.SeriesCollection[0].DataLabelsCollection.Add();			
			dl.HasValue=true;
			dl.HasPercentage=true;			
			//ͼ���ͼ����ͼ���������Ҳࡣ
//			dl.Position=Microsoft.Office.Interop.Owc11.ChartDataLabelPositionEnum.chLabelPositionRight;
			
			string filename=DateTime.Now.Ticks.ToString()+".gif";			
			string strAbsolutePath = _phaysicalimagepath + "\\"+filename;
			objCSpace.ExportPicture(strAbsolutePath, "GIF", _picwidth, _pichight);//�����GIF�ļ�.

			return filename;
		}

		/// <summary>
		/// ����ͼ
		/// </summary>
		/// <returns></returns>
		public string CreateBar()
		{	
			Microsoft.Office.Interop.Owc11.ChartSpace objCSpace = new Microsoft.Office.Interop.Owc11.ChartSpaceClass();//����ChartSpace����������ͼ��			
			Microsoft.Office.Interop.Owc11.ChChart objChart  = objCSpace.Charts.Add(0);//��ChartSpace���������ͼ��Add��������chart����
											
			//ָ��ͼ������͡�������OWC.ChartChartTypeEnumö��ֵ�õ�//Microsoft.Office.Interop.OWC.ChartChartTypeEnum
			objChart.Type=Microsoft.Office.Interop.Owc11.ChartChartTypeEnum.chChartTypeBarClustered;
			
			//ָ��ͼ���Ƿ���Ҫͼ��
			objChart.HasLegend = true;
			
			//����
			objChart.HasTitle = true;
			objChart.Title.Caption= _title;
//			objChart.Title.Font.Bold=true;
//			objChart.Title.Font.Color="blue";
								

			#region ��ʽ����		

//			//��ת
//			objChart.Rotation  = 360;//��ʾָ����άͼ�����ת�Ƕ�
//			objChart.Inclination = 10;//��ʾָ����άͼ�����ͼб�ʡ���Ч��ΧΪ -90 �� 90

			//������ɫ
//			objChart.PlotArea.Interior.Color = "red";

			//������ɫ
//			objChart.PlotArea.Floor.Interior.Color = "green";
// 
//			objChart.Overlap = 50;//��������б�־֮����ص���

			#endregion
			
			//x,y���ͼʾ˵��
			objChart.Axes[0].HasTitle = true;
			objChart.Axes[0].Title.Caption = "X �� ���";
			objChart.Axes[1].HasTitle = true;
			objChart.Axes[1].Title.Caption = "Y �� ����";
			

			//���һ��series
			Microsoft.Office.Interop.Owc11.ChSeries ThisChSeries = objChart.SeriesCollection.Add(0);


			//����series������
			ThisChSeries.SetData(Microsoft.Office.Interop.Owc11.ChartDimensionsEnum.chDimSeriesNames,
				Microsoft.Office.Interop.Owc11.ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),SeriesName);
			//��������
			ThisChSeries.SetData(Microsoft.Office.Interop.Owc11.ChartDimensionsEnum.chDimCategories,
				Microsoft.Office.Interop.Owc11.ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),strCategory);
			//����ֵ
			ThisChSeries.SetData(Microsoft.Office.Interop.Owc11.ChartDimensionsEnum.chDimValues,
				Microsoft.Office.Interop.Owc11.ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),strValue);

			Microsoft.Office.Interop.Owc11.ChDataLabels dl=objChart.SeriesCollection[0].DataLabelsCollection.Add();			
			dl.HasValue=true;
//			dl.Position=Microsoft.Office.Interop.Owc11.ChartDataLabelPositionEnum.chLabelPositionOutsideEnd;

						
			string filename=DateTime.Now.ToString("yyyyMMddHHmmssff")+".gif";
			string strAbsolutePath = _phaysicalimagepath + "\\"+filename;
			objCSpace.ExportPicture(strAbsolutePath, "GIF", _picwidth, _pichight);//�����GIF�ļ�.

			return filename;
			
		}



	}
}
