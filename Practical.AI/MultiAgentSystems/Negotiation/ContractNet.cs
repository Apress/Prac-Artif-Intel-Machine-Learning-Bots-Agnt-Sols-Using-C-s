using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Practical.AI.MultiAgentSystems.Communication;
using Practical.AI.MultiAgentSystems.Communication.Acl;

namespace Practical.AI.MultiAgentSystems.Negotiation
{
    public class ContractNet
    {

        public static IEnumerable<string> Announcement(CleaningTask cleaningTask, MasCleaningAgent manager, IEnumerable<MasCleaningAgent> contractors, FibaAcl language)
        {
            var tasks = cleaningTask.SubTasks;

            foreach (var contractor in contractors)
            {
                foreach (var task in tasks) 
                    language.Message(Performative.Cfp, manager.Id.ToString(), contractor.Id.ToString(), task);
            }

            return tasks;
        }

        public static void Bidding(IEnumerable<string> tasks, IEnumerable<MasCleaningAgent> contractors)
        {
             foreach (var contractor in contractors)
                contractor.Bid(tasks);
        }

        public static void Awarding(List<string> messages, MasCleaningAgent manager, IEnumerable<MasCleaningAgent> contractors, CleaningTask task, FibaAcl language)
        {
            var agentsAssigned = new List<Tuple<MasCleaningAgent, Tuple<int, int>>>();
            var messagesToDict = messages.ConvertAll(FibaAcl.MessagesToDict);

            // Processing bids
            foreach (var colRange in task.SubDivide)
            {
                var firstCol = colRange.Item1;
                var secondCol = colRange.Item2;
                var bidsFirstCol = new List<KeyValuePair<MasCleaningAgent, List<Tuple<double, Tuple<int, int>>>>>();
                var bidsSecondCol = new List<KeyValuePair<MasCleaningAgent, List<Tuple<double, Tuple<int, int>>>>>();

                foreach (var contractor in contractors)
                {
                    // Skip agents that have already been assigned
                    if (agentsAssigned.Exists(tuple => tuple.Item1.Id == contractor.Id))
                        continue;

                    var c = contractor;
                    // Get messages from current contractor
                    var messagesFromContractor = messagesToDict.FindAll(m => m.ContainsKey("from") && m["from"] == c.Id.ToString());

                    var bids = FibaAcl.GetContent(messagesFromContractor);
                    // Bids to first column in the range column
                    var bidsContractorFirstCol = bids.FindAll(b => b.Item2.Item2 == firstCol);
                    // Bids to second column in the range column
                    var bidsContractorSecondCol = bids.FindAll(b => b.Item2.Item2 == secondCol);

                    if (bidsContractorFirstCol.Count > 0)
                    {
                        bidsFirstCol.Add(
                            new KeyValuePair<MasCleaningAgent, List<Tuple<double, Tuple<int, int>>>>(contractor,
                                                                                                  bidsContractorFirstCol));
                    }
                    if (bidsContractorSecondCol.Count > 0)
                    {
                        bidsSecondCol.Add(
                            new KeyValuePair<MasCleaningAgent, List<Tuple<double, Tuple<int, int>>>>(contractor,
                                                                                                  bidsContractorSecondCol));
                    }
                }

                // Decide
                bidsFirstCol.Sort(Comparison);
                bidsSecondCol.Sort(Comparison);

                var closestAgentFirst = bidsFirstCol.FirstOrDefault();
                var closestAgentSecond = bidsSecondCol.FirstOrDefault();

                if (closestAgentFirst.Value != null)
                    closestAgentFirst.Value.Sort(Comparison);

                if (closestAgentSecond.Value != null)
                    closestAgentSecond.Value.Sort(Comparison);

                if (closestAgentFirst.Value != null && closestAgentSecond.Value != null)
                {
                    if (closestAgentFirst.Value.First().Item1 >= closestAgentSecond.Value.First().Item1)
                        agentsAssigned.Add(new Tuple<MasCleaningAgent, Tuple<int, int>>(closestAgentSecond.Key,
                                                                                     closestAgentSecond.Value.First().
                                                                                         Item2));
                    else
                        agentsAssigned.Add(new Tuple<MasCleaningAgent, Tuple<int, int>>(closestAgentFirst.Key,
                                                                                     closestAgentFirst.Value.First().
                                                                                         Item2));
                }
                else if (closestAgentFirst.Value == null)
                    agentsAssigned.Add(new Tuple<MasCleaningAgent, Tuple<int, int>>(closestAgentSecond.Key,
                                                                                     closestAgentSecond.Value.First().
                                                                                         Item2));
                else
                    agentsAssigned.Add(new Tuple<MasCleaningAgent, Tuple<int, int>>(closestAgentFirst.Key,
                                                                                     closestAgentFirst.Value.First().
                                                                                         Item2));
            }

            foreach (var assignment in agentsAssigned)
                language.Message(Performative.Inform, manager.Id.ToString(), 
                    assignment.Item1.Id.ToString(), "clean(" + assignment.Item2.Item1 + "," + assignment.Item2.Item2 + ")");
        }

        private static int Comparison(Tuple<double, Tuple<int, int>> tupleA, Tuple<double, Tuple<int, int>> tupleB)
        {
            if (tupleA.Item1 > tupleB.Item1)
                return 1;
            if (tupleA.Item1 < tupleB.Item1)
                return -1;
            return 0;
        }

        private static int Comparison(KeyValuePair<MasCleaningAgent, List<Tuple<double, Tuple<int, int>>>> bidsAgentA, KeyValuePair<MasCleaningAgent, List<Tuple<double, Tuple<int, int>>>> bidsAgentB)
        {
            if (bidsAgentA.Value.Min(p => p.Item1) > bidsAgentB.Value.Min(p => p.Item1))
                return 1;
            if (bidsAgentA.Value.Min(p => p.Item1) < bidsAgentB.Value.Min(p => p.Item1))
                return -1;
            return 0;
        }
    }

    public enum ContractRole
    {
        Contractor, Manager, None
    }
}
