using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;

namespace AO_ItemScript_Manager
{
	public static class Parsing
	{
		public static FlowDocument TryParsingScript(string content)
		{
			FlowDocument output = new FlowDocument();
			BrushConverter bc = new BrushConverter();
			string lastColor = "";

			List<string> actionList1 = new List<string>();
			List<string> actionList2 = new List<string>();

			// Check if there's a font or href
			if (content.Contains("href=\"text") || content.Contains("font color"))
			{
				foreach (var item in content.Split('<').ToList<string>())
				{
					actionList1 = item.Split('>').ToList<string>();

					foreach (var action in actionList1)
					{
						actionList2.Add(action);
					}
				}

				string[] actions = actionList2.ToArray();
				Paragraph tmpContent = new Paragraph();
				int titleIndex = 0;

				// Find our link text first
				for (int i = 0; i < actions.Length; i++)
				{
					// This should grab our link text, as the item right before the </a> marker
					// should be the clickable title that shows in chat. If there are 2 entries
					// with /a, then this should grab the last one (in the event there's an
					// additional link within the item)
					if (actions[i] == "/a")
						titleIndex = i - 1;
				}

				// actions now contains each step of what needs done
				for (int i = 0; i < actions.Length; i++)
				{
					// Make sure the entry isnt our title, ending </a>, \", or href code
					if (i == titleIndex
						|| i == titleIndex + 1
						|| actions[i].Contains("a href")
						|| actions[i] == "/a"
						|| actions[i] == "\\\""
						|| actions[i] == "/font")
						continue;

					// break/paragraph
					else if (actions[i] == "br" || actions[i] == "p" || actions[i] == "")
						continue;

					// font color
					else if (actions[i].Contains("font color"))
					{
						// grab our left/right entries
						string[] stringArray = actions[i].Split('=');

						// last element should be the color code
						lastColor = stringArray[stringArray.Length - 1];

						// Add our color to the paragraph
						tmpContent.Foreground = (Brush)bc.ConvertFromString(lastColor);
					}
					// Otherwise, should just be normal text
					// dump our paragraph into the FlowDocument, and reset
					// for the next cycle, continue our color and fix
					// spacing between lines
					else
					{
						tmpContent.Inlines.Add(actions[i]);
						output.Blocks.Add(tmpContent);
						tmpContent = new Paragraph();
						tmpContent.LineHeight = 1;
						if (lastColor != "")
							tmpContent.Foreground = (Brush)bc.ConvertFromString(lastColor);
					}
				}
			}
			else
				output.Blocks.Add(new Paragraph(new Run("[Unknown item or cannot parse]")));

			return output;
		}

		public static FlowDocument TryParsingMenu(string content)
		{
			FlowDocument output = new FlowDocument();
			BrushConverter bc = new BrushConverter();
			string lastColor = "";

			List<string> actionList1 = new List<string>();
			List<string> actionList2 = new List<string>();

			// Check if there's a font or href
			if (content.Contains("href=\"text") || content.Contains("font color"))
			{
				foreach (var item in content.Split('<').ToList<string>())
				{
					actionList1 = item.Split('>').ToList<string>();
					bool isLink = false;

					foreach (var action in actionList1)
					{
						// Replace our link with [Link] and then the actual link text
						if (isLink)
						{
							actionList2.Add($"[Link]{action}");
							isLink = false;
						}
						// Move on, ignore font close
						else if (action == "/font")
							continue;
						// Mark our link for the next round
						else if (action.Contains("a href=\"chatcmd"))
						{
							isLink = true;
						}
						// If we're not ending a link or new line, add the text
						else if (!action.Contains("/a"))
							actionList2.Add(action);
					}
				}

				// Reuse our actionList1 and combine lines until new line pops up
				string s = string.Empty;
				actionList1.Clear();

				foreach (var item in actionList2)
				{
					if (item == "\n" || item == "\n\r" || item == "\r\n")
						s += "\n";

					else if (item.Contains("font color"))
					{
						// Get our currently combined string into the list first on it's own
						// and clear it for the next batch
						actionList1.Add(s);
						s = string.Empty;
						// Keep our font commands if they exist, in their own row
						actionList1.Add(item);
					}

					// Otherwise we keep adding to the string
					else
						s += item;
				}

				// If we have remaining text to add
				if (s != string.Empty)
					actionList1.Add(s);

				string[] actions = actionList1.ToArray();
				Paragraph tmpContent = new Paragraph();

				// actions now contains each step of what needs done
				for (int i = 0; i < actions.Length; i++)
				{
					// font color
					if (actions[i].Contains("font color"))
					{
						// grab our left/right entries
						string[] stringArray = actions[i].Split('=');

						// last element should be the color code
						lastColor = stringArray[stringArray.Length - 1];

						// Add our color to the paragraph
						tmpContent.Foreground = (Brush)bc.ConvertFromString(lastColor);
					}
					// Otherwise, should just be normal text
					// dump our paragraph into the FlowDocument, and reset
					// for the next cycle, continue our color and fix
					// spacing between lines
					else
					{
						tmpContent.Inlines.Add(actions[i]);
						output.Blocks.Add(tmpContent);
						tmpContent = new Paragraph();
						tmpContent.LineHeight = 1;
						if (lastColor != "")
							tmpContent.Foreground = (Brush)bc.ConvertFromString(lastColor);
					}
				}
			}
			else
				output.Blocks.Add(new Paragraph(new Run("[Unknown item or cannot parse]")));

			return output;
		}
	}
}
