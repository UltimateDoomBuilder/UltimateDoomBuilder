using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BlockmapExplorer.Controls
{
	internal partial class BlockmapExplorerDocker : UserControl
	{
		internal CheckBox ShowQuestionableBlocks => cbShowQuestionableBlocks;

		internal BlockmapExplorerDocker()
		{
			InitializeComponent();
		}

		internal void SetBlockInfo(int column, int row, BlockmapData blockmapData)
		{
			lblColumn.Text = column.ToString();
			lblRow.Text = row.ToString();
			lblOffset.Text = blockmapData.BlockPointers[column, row].ToString();
			lblNumLinesInBlock.Text = blockmapData.GetLinesInBlock(column, row).Count.ToString();
			lblIsSublist.Text = blockmapData.Blocks[blockmapData.BlockPointers[column, row]]?.IsSublist == true ? "Yes" : "No";
		}

		internal void SetInfo(int totalBlocks, int uniqueBlocks, int questionableOffsetsCount, int columns, int rows, int linesNotInBlocks, long lumpSize)
		{
			lblTotalColumns.Text = columns.ToString();
			lblTotalRows.Text = rows.ToString();

			lblTotalBlocks.Text = totalBlocks.ToString();
			lblUniqueBlocks.Text = uniqueBlocks.ToString();

			lblLinesNotInBlocks.Text = linesNotInBlocks.ToString();

			lblLumpSize.Text = lumpSize.ToString();
			lblOffsetListEnd.Text = (8 + totalBlocks * 2).ToString();

			lblQuestionableOffsets.Visible = cbShowQuestionableBlocks.Visible = questionableOffsetsCount > 0;
			lblQuestionableOffsets.Text = $"There are {questionableOffsetsCount} offset{(questionableOffsetsCount == 1 ? "" : "s")} pointing into the header or offset list, which indicates problems with the blockmap, for example exceeding the maximum size";
		}

		internal void ClearBlockInfo()
		{
			lblColumn.Text = "-";
			lblRow.Text = "-";
			lblOffset.Text = "-";
			lblNumLinesInBlock.Text = "-";
			lblIsSublist.Text = "-";
		}
	}
}
