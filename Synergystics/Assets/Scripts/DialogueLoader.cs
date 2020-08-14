using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueLoader
{
    public class DialogueVariants
    {
        public int EventID;
        public bool FocusState;
        public AdvisorType ForAdvisor;
        public List<DialogueOptionContainer> Variants;       
    }

    public class DialogueOptionContainer
    {
        public Advisor.AdvisorTraits TraitWeightings;
        public string Variant;      

        // Returns true if the advisor trait rolls match against TraitWeightings?
        public bool Matches(Advisor advisor)
        {
            // If this has empty weightings, it is the 'default' option
            if(TraitWeightings.Empty())
            {
                return true;
            }

            // TODO
            return true;
        }
    }

    // List of all spoken variants
    private List<DialogueVariants> AllDialogueVariants = new List<DialogueVariants>();

    // List of all response variants
    private List<DialogueVariants> AllResponseVariants = new List<DialogueVariants>();

    public DialogueLoader(string resourcePath)
    {
        // TODO
    }

    public string FindDialogue(GameEvent relatedEvent, Advisor advisor, bool isFocusSelected, bool isResponse)
    {
        // Get the list containing what selection we want
        List<DialogueVariants> selection = isResponse ? AllResponseVariants : AllDialogueVariants;

        // Find the variant that matches our criteria
        DialogueVariants variant = selection.Find(x => x.EventID == relatedEvent.EventID && x.FocusState == isFocusSelected && (x.ForAdvisor == AdvisorType.DEFAULT || x.ForAdvisor == advisor.GetAdvisorType()));
       
        // Return an error if we can't find one 
        if(variant == null)
        {
            return "An error occurred while finding a matching variant";
        }
        
        // Get a 'list' of any matches (i.e. legitimate matching options & default variants)
        IEnumerable<DialogueOptionContainer> finalSelection = variant.Variants.Where(x => x.Matches(advisor)); 

        // Return an error if there were no matches
        if(finalSelection == null || finalSelection.Count() == 0)
        {
            return "An error occurred while making the final variant selection";
        }
        
        if(finalSelection.Count() == 1)
        {
            // If there is only one selection then return it
            return finalSelection.First().Variant;
        } else
        {
            // If there are multiple selections (i.e. a default & at least one other legitimate trait match), return the trait match
            IEnumerable<DialogueOptionContainer> traitMatchContainers = finalSelection.Where(x => !x.TraitWeightings.Empty());
            
            if(traitMatchContainers.Count() == 0)
            {
                // If we couldn't find a trait match, then return the first default variant
                return finalSelection.First().Variant;
            } else
            {
                // Return the first trait match variant regardless of how many others there were (may be subject to change?)
                return traitMatchContainers.First().Variant;
            }
        }       
    }
}
