﻿using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EventBuilder
{ 
    // Spreadsheet data for actions
    public class StorySpreadsheetDataSubset
    {
        public string AdvisorType;
        public string AdvisorSubtype;
        public int SubtypeCol;

        public string PrefocusAdvisorOpinion;

        public string Action1Opinion;
        public string Action2Opinion;
        public string Action3Opinion;
        public string Action4Opinion;
    }

    // Story data from spreadsheet
    public class StoryData
    {
        public string StoryTitle;
        public string StoryDescriptor;
        public bool IsValidStory;
        public string FocusOutcome;
        public string MapSource;
        public string EventSummary;

        public string Action1;
        public string Action2;
        public string Action3;
        public string Action4;

        public string ActionSummary1;
        public string ActionSummary2;
        public string ActionSummary3;
        public string ActionSummary4;

        public List<StorySpreadsheetDataSubset> AdvisorData;
    }

    // Template data for top-level json
    public class TopLevelTemplate
    {
        public int EventID;
        public string EventName;
        public string MapSource;
        public bool IsValidStory;
        public string StoryDescriptor;
        public string OutcomeDescriptor;
        public string StoryTitle;
        public string ShortOutcomeDescriptor;
        public string EventSummary;

        public string PreSelectionPrefix;
        public string SolutionOpinionPrefix;
        public string EventSolutionPrefix;

        public string DataFolder;
    }

    // Pre-focus advisor opinion
    public class PreSelectionTemplate
    {
        public string AdvisorType;
        public string Opinion;
    }

    // Event solution data
    public class EventSolutionTemplate
    {
        public int SolutionIndex;
        public string ActionDescription;
        public string ActionSummary;
    }

    // Post-focus advisor opinion & opinions for each solution
    public class AfterSelectionTemplate
    {
        public string AdvisorType;
        public string[] SolutionOpinions;
    }

    // Actual builder class
    public class EventBuilder
    {
        public static void Main(string[] args)
        {
            // Usage
            if(args.Length != 6)
            {
                Console.WriteLine("usage: resource_location event_name solution_prefix prefocus_prefix postfocus_prefix input_file");
                return;
            }

            // Get data from args
            string resourceFolderLocation = args[0];
            string eventName = args[1];
            string solutionPrefix = args[2];
            string preFocusPrefix = args[3];
            string postFocusPrefix = args[4];
            string inputFileLocation = args[5];

            // Read story data
            StoryData data = GetStoryData(inputFileLocation);

            // Check how many existing files there are to use as our eventID
            int existingFiles = Directory.GetFiles($"{resourceFolderLocation}\\", "*.json").Length;
            Console.WriteLine($"Found {existingFiles} files...");

            // Create top level template
            TopLevelTemplate topLevelTemplate = new TopLevelTemplate()
            {
                DataFolder = $"{eventName}_data",
                EventID = existingFiles,
                EventName = eventName,
                StoryTitle = data.StoryTitle,
                EventSolutionPrefix = solutionPrefix,
                IsValidStory = data.IsValidStory,
                MapSource = data.MapSource,
                OutcomeDescriptor = data.FocusOutcome,
                PreSelectionPrefix = preFocusPrefix,
                ShortOutcomeDescriptor = data.FocusOutcome,
                SolutionOpinionPrefix = postFocusPrefix,
                StoryDescriptor = data.StoryDescriptor,
                EventSummary = data.EventSummary
            };

            // Write out top level data and create data folder
            File.WriteAllText($"{resourceFolderLocation}\\{eventName}.json", JsonConvert.SerializeObject(topLevelTemplate, Formatting.Indented));
            Directory.CreateDirectory($"{resourceFolderLocation}\\{topLevelTemplate.DataFolder}");

            // Write out opinion subset data
            foreach(StorySpreadsheetDataSubset subset in data.AdvisorData)
            {
                PreSelectionTemplate preTemplate = new PreSelectionTemplate()
                {
                    AdvisorType = subset.AdvisorType,
                    Opinion = subset.PrefocusAdvisorOpinion
                };

                AfterSelectionTemplate afterTemplate = new AfterSelectionTemplate()
                {
                    AdvisorType = subset.AdvisorType,
                    SolutionOpinions = new string[] { subset.Action1Opinion, subset.Action2Opinion, subset.Action3Opinion, subset.Action4Opinion }
                };

                File.WriteAllText($"{resourceFolderLocation}\\{topLevelTemplate.DataFolder}\\{topLevelTemplate.PreSelectionPrefix}_{subset.AdvisorType}{subset.AdvisorSubtype}.json", JsonConvert.SerializeObject(preTemplate, Formatting.Indented));
                File.WriteAllText($"{resourceFolderLocation}\\{topLevelTemplate.DataFolder}\\{topLevelTemplate.SolutionOpinionPrefix}_{subset.AdvisorType}{subset.AdvisorSubtype}.json", JsonConvert.SerializeObject(afterTemplate, Formatting.Indented));
            }

            // Create 'solutions'
            EventSolutionTemplate[] solutions = new EventSolutionTemplate[]
            {
                new EventSolutionTemplate()
                {
                    SolutionIndex = 0,
                    ActionDescription = data.Action1,
                    ActionSummary = data.ActionSummary1
                },
                new EventSolutionTemplate()
                {
                    SolutionIndex = 1,
                    ActionDescription = data.Action2,
                    ActionSummary = data.ActionSummary2
                },
                new EventSolutionTemplate()
                {
                    SolutionIndex = 2,
                    ActionDescription = data.Action3,
                    ActionSummary = data.ActionSummary3
                }, 
                new EventSolutionTemplate()
                {
                    SolutionIndex = 3,
                    ActionDescription = data.Action4,
                    ActionSummary = data.ActionSummary4
                }
            };

            // Write all solution data
            foreach(EventSolutionTemplate template in solutions)
            {
                File.WriteAllText($"{resourceFolderLocation}\\{topLevelTemplate.DataFolder}\\{topLevelTemplate.EventSolutionPrefix}_Solution{template.SolutionIndex}.json", JsonConvert.SerializeObject(template, Formatting.Indented));
            }            
        }

        // Function to read story data
        private static StoryData GetStoryData(string inputFileLocation)
        {
            StoryData data = new StoryData();
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(inputFileLocation)))
            {
                var storySheet = package.Workbook.Worksheets[0];

                // Get base cells
                var baseSubtypeCell = storySheet.Cells["A2"];
                var baseMasterCell = storySheet.Cells["A1"];

                List<StorySpreadsheetDataSubset> advisorData = new List<StorySpreadsheetDataSubset>();

                string subtypeCellValue = null;
                string masterTypeCellValue = null;

                do
                {
                    // Read next value in cell (1 col down)
                    baseSubtypeCell = storySheet.Cells[baseSubtypeCell.Start.Row, baseSubtypeCell.Start.Column + 1];
                    baseMasterCell = storySheet.Cells[baseMasterCell.Start.Row, baseMasterCell.Start.Column + 1];

                    // Get text
                    subtypeCellValue = baseSubtypeCell.Text;
                    string newMasterValue = baseMasterCell.Text;
                    if (newMasterValue != null && !newMasterValue.Equals(string.Empty))
                    {
                        // If the mastervalue is valid, use it
                        masterTypeCellValue = newMasterValue;
                    }

                    if (!subtypeCellValue.Equals(string.Empty) && !masterTypeCellValue.Equals(string.Empty))
                    {
                        // Create a new subset class, use the master type as the advisor type, also use the subtype cell for traits
                        advisorData.Add(new StorySpreadsheetDataSubset()
                        {
                            AdvisorType = masterTypeCellValue,
                            AdvisorSubtype = subtypeCellValue,
                            SubtypeCol = baseSubtypeCell.Start.Column                          
                        });
                    }
                } while (subtypeCellValue != string.Empty);

                // Find action opinions for each advisor
                foreach(StorySpreadsheetDataSubset subset in advisorData)
                {
                    subset.PrefocusAdvisorOpinion = storySheet.Cells[5, subset.SubtypeCol].Text;

                    subset.Action1Opinion = storySheet.Cells[8, subset.SubtypeCol].Text;
                    subset.Action2Opinion = storySheet.Cells[10, subset.SubtypeCol].Text;
                    subset.Action3Opinion = storySheet.Cells[12, subset.SubtypeCol].Text;
                    subset.Action4Opinion = storySheet.Cells[14, subset.SubtypeCol].Text;
                }

                // Some hardcoded template stuff that is always in the same location
                data.StoryTitle = storySheet.Cells[3, 2].Text;
                data.StoryDescriptor = storySheet.Cells[4, 2].Text;
                data.FocusOutcome = storySheet.Cells[6, 2].Text;
                data.IsValidStory = storySheet.Cells[4, 1].GetValue<bool>();
                data.MapSource = storySheet.Cells[3, 1].Text;
                data.EventSummary = storySheet.Cells[15, 2].Text;
                data.Action1 = storySheet.Cells[7, 2].Text;
                data.Action2 = storySheet.Cells[9, 2].Text;
                data.Action3 = storySheet.Cells[11, 2].Text;
                data.Action4 = storySheet.Cells[13, 2].Text;
                data.ActionSummary1 = storySheet.Cells[16, 2].Text;
                data.ActionSummary2 = storySheet.Cells[17, 2].Text;
                data.ActionSummary3 = storySheet.Cells[18, 2].Text;
                data.ActionSummary4 = storySheet.Cells[19, 2].Text;

                data.AdvisorData = advisorData;
            }

            // Return completed story data
            return data;
        }
    }
}
