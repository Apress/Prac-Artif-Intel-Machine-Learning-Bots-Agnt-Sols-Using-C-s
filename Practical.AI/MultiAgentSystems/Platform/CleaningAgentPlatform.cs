using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Practical.AI.MultiAgentSystems.Communication;
using Practical.AI.MultiAgentSystems.Negotiation;

namespace Practical.AI.MultiAgentSystems.Platform
{
    public class CleaningAgentPlatform
    {
        public Dictionary<Guid, MasCleaningAgent> Directory { get; set; }
        public IEnumerable<MasCleaningAgent> Agents { get; set; }
        public IEnumerable<MasCleaningAgent> Contractors { get; set; }
        public MasCleaningAgent Manager { get; set; }
        public CleaningTask Task { get; set; }

        public CleaningAgentPlatform(IEnumerable<MasCleaningAgent> agents, CleaningTask task)
        {
            Agents = new List<MasCleaningAgent>(agents);
            Directory = new Dictionary<Guid, MasCleaningAgent>();
            Task = task;

            foreach (var cleaningAgent in Agents)
            {
                Directory.Add(cleaningAgent.Id, cleaningAgent);
                cleaningAgent.Platform = this;
            }

            DecideRoles();
            
        }

        public void DecideRoles()
        {
            // Manager Role
            Manager = Agents.First(a => a.CleanedCells.Count == Agents.Max(p => p.CleanedCells.Count));
            Manager.Role = ContractRole.Manager;
            // Contract Roles
            Contractors = new List<MasCleaningAgent>(Agents.Where(a => a.Id != Manager.Id));
            foreach (var cleaningAgent in Contractors)
                cleaningAgent.Role = ContractRole.Contractor;
            (Contractors as List<MasCleaningAgent>).Add(Manager);
        }
    }
}
