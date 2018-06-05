using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.Logic
{
    /// <summary>
    /// Abstract class representing a formula in propositionallogic
    /// </summary>
    public abstract class Formula
    {
         /// <summary>
         /// Evaluates the formula assuming all variables have values true, false defined
         /// </summary>
         /// <returns>Returns the result of the formula after such assignment</returns>
         public abstract bool Evaluate();
         /// <summary>
         /// Finds every variable in a formula
         /// </summary>
         /// <returns>Returns an IEnumerable of variables</returns>
         public abstract IEnumerable<Variable> Variables();
         /// <summary>
         /// Finds every literal in a formula
         /// </summary>
         /// <returns>Returns an IEnumerable of literals</returns>
         public abstract IEnumerable<Formula> Literals();
         /// <summary>
         /// Transforms a formula into Negation Normal Form (NNF)
         /// </summary>
         /// <returns>Returns a new formuna in NNF</returns>
         public abstract Formula ToNnf();
         /// <summary>
         /// Transforms a formula into Conjunctive Normal Form (CNF)
         /// </summary>
         /// <returns>Returns a new formuna in CNF</returns>
         public abstract Formula ToCnf();
         
         /// <summary>
         /// Distributes a formula using Morgan's laws of propositional logic
         /// </summary>
         /// <param name="p"></param>
         /// <param name="q"></param>
         /// <returns>The formula distribuited</returns>
         public Formula DistributeCnf(Formula p, Formula q)
         {
             if (p is And)
                 return new And(DistributeCnf((p as And).P, q), DistributeCnf((p as And).Q, q));
             if (q is And)
                 return new And(DistributeCnf(p, (q as And).P), DistributeCnf(p, (q as And).Q));

             return new Or(p, q);
         }

    }

    /// <summary>
    /// Abstract class for Binary logical connectives (AND, OR)
    /// </summary>
    public abstract class BinaryGate : Formula
    {
        public Formula P { get; set; }
        public Formula Q { get; set; }

        protected BinaryGate(Formula p, Formula q)
        {
            P = p;
            Q = q;
        }

        public override IEnumerable<Variable> Variables()
        {
            return P.Variables().Concat(Q.Variables());
        }

        public override IEnumerable<Formula> Literals()
        {
            return P.Literals().Concat(Q.Literals());
        }
    }

    /// <summary>
    /// Represents the conjunction of two formulas
    /// </summary>
    public class And: BinaryGate
    {

        public And(Formula p, Formula q): base(p, q)
        { }

        public override bool Evaluate()
        {
            return P.Evaluate() && Q.Evaluate();
        }

        public override Formula ToNnf()
        {
            return new And(P.ToNnf(), Q.ToNnf());
        }

        public override Formula ToCnf()
        {
            return new And(P.ToCnf(), Q.ToCnf());
        }

        public override string ToString()
        {
            return "(" + P + " & " + Q + ")";
        }

    }

    /// <summary>
    /// Represents the disjunction of two formulas
    /// </summary>
    public class Or : BinaryGate
    {
        public Or(Formula p, Formula q): base(p, q)
        { }

        public override bool Evaluate()
        {
            return P.Evaluate() || Q.Evaluate();
        }

        public override Formula ToNnf()
        {
            return new Or(P.ToNnf(), Q.ToNnf());
        }

        public override Formula ToCnf()
        {
            return DistributeCnf(P.ToCnf(), Q.ToCnf());
        }

        public override string ToString()
        {
            return "(" + P + " | " + Q + ")";
        }
    }

    /// <summary>
    /// Represents the negation of a formula
    /// </summary>
    public class Not : Formula
    {
        public Formula P { get; set; }

        public Not(Formula p)
        {
            P = p;
        }

        public override bool Evaluate()
        {
            return !P.Evaluate();
        }

        public override IEnumerable<Variable> Variables()
        {
            return new List<Variable>(P.Variables());
        }

        public override Formula ToNnf()
        {
            if (P is And)
                return new Or(new Not((P as And).P).ToNnf(), new Not((P as And).Q).ToNnf());
            if (P is Or)
                return new And(new Not((P as Or).P).ToNnf(), new Not((P as Or).Q).ToNnf());
            if (P is Not)
                return (P as Not).P.ToNnf();

            return this;
        }

        public override Formula ToCnf()
        {
            return this;
        }

        public override IEnumerable<Formula> Literals()
        {
            return P is Variable ? new List<Formula>() { this } : P.Literals();
        }

        public override string ToString()
        {
            return "!" + P;
        }
    }

    /// <summary>
    /// Represents a variable
    /// </summary>
    public class Variable : Formula
    {
        public bool Value { get; set; }
        public string Name { get; set; }

        public Variable(bool value)
        {
            Value = value;
        }

        public override bool Evaluate()
        {
            return Value;
        }

        public override IEnumerable<Variable> Variables()
        {
            return new List<Variable> { this };
        }

        public override Formula ToNnf()
        {
            return this;
        }

        public override Formula ToCnf()
        {
            return this;
        }

        public override IEnumerable<Formula> Literals()
        {
            return new List<Formula> { this };
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Represents a clause, i.e. a disjunction of literals
    /// </summary>
    public class Clause
    {
        public List<Formula> Literals { get; set; } 

        public Clause()
        {
            Literals = new List<Formula>();
        }

        /// <summary>
        /// Determines whether this clause contains a given literal
        /// </summary>
        /// <param name="literal"></param>
        /// <returns>True if it contains literal; false otherwise</returns>
        public bool Contains(Formula literal)
        {
            if (!IsLiteral(literal))
                throw new ArgumentException("Specified formula is not a literal");

            foreach (var formula in Literals)
            {
                if (LiteralEquals(formula, literal))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a literal from this clause.
        /// </summary>
        /// <param name="literal"></param>
        /// <returns>Returns a new clause with the given literal removed</returns>
        public Clause RemoveLiteral(Formula literal)
        {
            if (!IsLiteral(literal))
                throw new ArgumentException("Specified formula is not a literal");

            var result = new Clause();

            for (var i = 0; i < Literals.Count; i++)
            {
                if (!LiteralEquals(literal, Literals[i]))
                    result.Literals.Add(Literals[i]);
            }

            return result;
        }

        /// <summary>
        /// Determines whether literals p, q are equal
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns>True if p and q are equal; false otherwise</returns>
        public bool LiteralEquals(Formula p, Formula q)
        {
            if (p is Variable && q is Variable)
                return (p as Variable).Name == (q as Variable).Name;
            if (p is Not && q is Not)
                return LiteralEquals((p as Not).P, (q as Not).P);

            return false;
        }

        public bool IsLiteral(Formula p)
        {
            return p is Variable || (p is Not && (p as Not).P is Variable);
        }
    }

    /// <summary>
    /// Represents a Conjunctive Normal Form, i.e. a conjunction of clauses
    /// </summary>
    public class Cnf
    {
        public List<Clause> Clauses { get; set; } 

        public Cnf()
        {
            Clauses = new List<Clause>();
        }

        public Cnf(And and)
        {
            Clauses = new List<Clause>();
            RemoveParenthesis(and);
        }

        /// <summary>
        /// Simplifies a formula by eliminating clauses containing a literal and its negation.
        /// </summary>
        public void SimplifyCnf()
        {
            Clauses.RemoveAll(TautologyClauses);
        }

        /// <summary>
        /// Determines whether a clause contains a literal and its negation
        /// </summary>
        /// <param name="clause"></param>
        /// <returns>True if a literal and its negation belong to clause; false otherwise.</returns>
        private bool TautologyClauses(Clause clause)
        {
            for (var i = 0; i < clause.Literals.Count; i++)
            {
                for (var j = i + 1; j < clause.Literals.Count - 1; j++)
                {
                    // Checking that literal i and literal j are not of the same type i.e. both variables or negated literals.
                    if (!(clause.Literals[i] is Variable && clause.Literals[j] is Variable) &&
                        !(clause.Literals[i] is Not && clause.Literals[j] is Not))
                    {
                        var not = clause.Literals[i] is Not ? clause.Literals[i] as Not : clause.Literals[j] as Not;
                        var @var = clause.Literals[i] is Variable ? clause.Literals[i] as Variable : clause.Literals[j] as Variable;
                        if (IsNegation(not, @var))
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether a formula f1 is the negation of a formula f2
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns>True if f1 is the negation of f2; false otherwise</returns>
        private bool IsNegation(Not f1, Variable f2)
        {
            return (f1.P as Variable).Name == f2.Name;
        }

        /// <summary>
        /// Adds new clauses
        /// </summary>
        /// <param name="others"></param>
        private void Join(IEnumerable<Clause> others)
        {
            Clauses.AddRange(others);
        }

        /// <summary>
        /// Removes all parenthesis of an AND formula leaving it as a list of clauses
        /// </summary>
        /// <param name="and"></param>
        /// <returns>Returns a list of clauses</returns>
        private void RemoveParenthesis(And and)
        {
            var currentAnd = and;

            while (true)
            {
                // If P is OR or literal and Q is Or or literal.
                if ((currentAnd.P is Or || currentAnd.P is Variable || currentAnd.P is Not) &&
                    (currentAnd.Q is Or || currentAnd.Q is Variable || currentAnd.Q is Not))
                {
                    Clauses.Add(new Clause { Literals = new List<Formula>(currentAnd.P.Literals()) });
                    Clauses.Add(new Clause { Literals = new List<Formula>(currentAnd.Q.Literals()) });
                    break;
                }
                // If P is AND and Q is OR or literal.
                if (currentAnd.P is And && (currentAnd.Q is Or || currentAnd.Q is Variable || currentAnd.Q is Not))
                {
                    Clauses.Add(new Clause { Literals = new List<Formula>(currentAnd.Q.Literals()) });
                    currentAnd = currentAnd.P as And;
                }
                // If P is OR or literal and Q is AND.
                if ((currentAnd.P is Or || currentAnd.P is Variable || currentAnd.P is Not) && currentAnd.Q is And)
                {
                    Clauses.Add(new Clause { Literals = new List<Formula>(currentAnd.P.Literals()) });
                    currentAnd = currentAnd.Q as And;
                }
                // If both P and Q are ANDs.
                if (currentAnd.P is And && currentAnd.Q is And)
                {
                    RemoveParenthesis(currentAnd.P as And);
                    RemoveParenthesis(currentAnd.Q as And);
                    break;
                }
            }
        }

        #region DPLL

        /// <summary>
        /// DPLL algorithm
        /// </summary>
        /// <returns>True if the formula is SAT; false otherwise</returns>
        public bool Dpll()
        {
            return Dpll(new Cnf {Clauses = new List<Clause>(Clauses)});
        }

        /// <summary>
        /// Recursive version of the algorithm
        /// </summary>
        /// <param name="cnf"></param>
        /// <returns>True if the formula is SAT; false otherwise</returns>
        private bool Dpll(Cnf cnf)
        {
            // The CNF with no clauses is assumed to be True
            if (cnf.Clauses.Count == 0)
                return true;

            // Rule One Literal: if there exists a clause with a single literal
            // we assign it True and remove every clause containing it.
            var cnfAfterOneLit = OneLiteral(cnf);

            if (cnfAfterOneLit.Item2 == 0)
                return true;

            if (cnfAfterOneLit.Item2 < 0)
                return false;

            cnf = cnfAfterOneLit.Item1;

            // Rule Pure Literal: if there exists a literal and its negation does not exist in any clause of Cnf
            var cnfPureLit = PureLiteralRule(cnf);

            // Rule Split: splitting occurs over a literal and creates 2 branches of the tree
            var split = Split(cnfPureLit);

            return Dpll(split.Item1) || Dpll(split.Item2);
        }

        /// <summary>
        /// Splits the tree into a branch containing clauses where P appears and another branch with clauses where Not(p) appears. 
        /// </summary>
        /// <param name="cnf"></param>
        /// <returns>Returns a Tuple with two CnFs one for the first branch and another for the second branch.</returns>
        private Tuple<Cnf, Cnf> Split(Cnf cnf)
        {
            var literal = Heuristics.ChooseLiteral(cnf);
            if (literal == null)
                return new Tuple<Cnf, Cnf>(new Cnf(), new Cnf());
            var tuple = SplittingOnLiteral(cnf, literal);

            return new Tuple<Cnf, Cnf>(RemoveLiteral(tuple.Item1, literal), RemoveLiteral(tuple.Item2, NegateLiteral(literal)));
        }

        /// <summary>
        /// Removes a literal from the Cnf
        /// </summary>
        /// <param name="cnf"></param>
        /// <param name="literal"></param>
        /// <returns>A new Cnf with the literal removed</returns>
        private Cnf RemoveLiteral(Cnf cnf, Formula literal)
        {
            var result = new Cnf();

            foreach (var clause in cnf.Clauses)
                result.Clauses.Add(clause.RemoveLiteral(literal));
            
            return result;
        } 

        /// <summary>
        /// Splits the Cnf on a given literal
        /// </summary>
        /// <param name="cnf"></param>
        /// <param name="literal"></param>
        /// <returns>A tuple of Cnfs one for the left branch another for the right branch</returns>
        private Tuple<Cnf, Cnf> SplittingOnLiteral(Cnf cnf, Formula literal)
        {
            // List of clauses containing literal
            var @in = new List<Clause>();
            // List of clauses containing Not(literal)
            var @inNegated = new List<Clause>();
            // List of clauses not containing literal nor Not(literal)
            var @out = new List<Clause>();
            var negated = NegateLiteral(literal);

            foreach (var clause in cnf.Clauses)
            {
                if (clause.Contains(literal))
                    @in.Add(clause);
                else if (clause.Contains(negated))
                    @inNegated.Add(clause);
                else
                    @out.Add(clause);
            }

            var inCnf = new Cnf { Clauses = @in };
            var outCnf = new Cnf { Clauses = @inNegated };
            inCnf.Join(@out);
            outCnf.Join(@out);

            return new Tuple<Cnf, Cnf>(inCnf, outCnf);
        }

        /// <summary>
        /// Applies the pure literal rule, i.e. finds a literal such Not(literal) does not belong to Cnf.
        /// </summary>
        /// <param name="cnf"></param>
        /// <returns>Returns a new Cnf whose clauses do not contain any pure literal</returns>
        private Cnf PureLiteralRule(Cnf cnf)
        {
            var pureLiterals = PureLiterals(cnf);
            if (pureLiterals.Count() == 0)
                return cnf;

            var newCnf = new Cnf();
            var clausulesRemoved = new SortedSet<int>();

            // Checking what clausules contain pure literals
            foreach (var pureLiteral in pureLiterals)
            {
                for (var i = 0; i < cnf.Clauses.Count; i++)
                {
                    if (cnf.Clauses[i].Contains(pureLiteral))
                        clausulesRemoved.Add(i);
                }  
            }

            // Creating the new set of Clausules
            for (var i = 0; i < cnf.Clauses.Count; i++)
            {
                if (!clausulesRemoved.Contains(i))
                    newCnf.Clauses.Add(cnf.Clauses[i]);
            }

            return newCnf;
        }

        /// <summary>
        /// Gets all pure literals from Cnf
        /// </summary>
        /// <param name="cnf"></param>
        /// <returns>Returns a list containing all pure literals of Cnf</returns>
        private IEnumerable<Formula> PureLiterals(Cnf cnf)
        {
            var result = new List<Formula>();

            foreach (var clause in cnf.Clauses)
                foreach (var literal in clause.Literals)
                {
                    if (PureLiteral(cnf, literal))
                        result.Add(literal);
                }

            return result;
        }

        /// <summary>
        /// Determines whether a given literal is pure
        /// </summary>
        /// <param name="cnf"></param>
        /// <param name="literal"></param>
        /// <returns>True is literal is pure; false otherwise</returns>
        private bool PureLiteral(Cnf cnf, Formula literal)
        {
            var negation = NegateLiteral(literal);

            foreach (var clause in cnf.Clauses)
            {
                foreach (var l in clause.Literals)
                    if (clause.LiteralEquals(l, negation))
                        return false;
            }

            return true;
        }

        /// <summary>
        /// Applies the one literal rule i.e. finds all literals that are the single member of their clause.
        /// </summary>
        /// <param name="cnf"></param>
        /// <returns>Returns a tuple with the resulting Cnf after having removed clauses containing one literals or their negations and an integer which can be 0 (no clauses), 1 (continue dpll), -1 (empty clause)</returns>
        private Tuple<Cnf, int> OneLiteral(Cnf cnf)
        {
            var unitLiteral = UnitClause(cnf);
            if (unitLiteral == null)
                return new Tuple<Cnf, int>(cnf, 1);

            var newCnf = new Cnf();

            while (unitLiteral != null)
            {
                var clausesToRemove = new List<int>();
                var i = 0;

                // Finding clauses where the unit literal is, these clauses will be deleted
                foreach (var clause in cnf.Clauses)
                {
                    if (clause.Literals.Any(literal => clause.LiteralEquals(literal, unitLiteral)))
                        clausesToRemove.Add(i);
                    i++;
                }

                // New Cnf after removing every clause where unit literal is
                newCnf = new Cnf();

                // Leave clausules that do not include the unit literal
                for (var j = 0; j < cnf.Clauses.Count; j++)
                {
                    if (!clausesToRemove.Contains(j))
                        newCnf.Clauses.Add(cnf.Clauses[j]);
                }
                // No clauses which implies SAT
                if (newCnf.Clauses.Count == 0)
                    return new Tuple<Cnf, int>(newCnf, 0);

                // Remove negation of unit literal from remaining clauses
                var unitNegated = NegateLiteral(unitLiteral);
                var clausesNoLitNeg = new List<Clause>();

                foreach (var clause in newCnf.Clauses)
                {
                    var newClause = new Clause();

                    // Leaving every literal except the unit literal negated 
                    foreach (var literal in clause.Literals)
                        if (!clause.LiteralEquals(literal, unitNegated))
                            newClause.Literals.Add(literal);

                    clausesNoLitNeg.Add(newClause);
                }

                newCnf.Clauses = new List<Clause>(clausesNoLitNeg);
                // Resetting variables for next stage
                cnf = newCnf;
                unitLiteral = UnitClause(cnf);
                // Empty clause found
                if (cnf.Clauses.Any(c => c.Literals.Count == 0))
                    return new Tuple<Cnf, int>(newCnf, -1);
            }

            return new Tuple<Cnf, int>(newCnf, 1);
        }

        /// <summary>
        /// Negates a literal.
        /// </summary>
        /// <param name="literal"></param>
        /// <returns>Returns the negation of a literal</returns>
        public Formula NegateLiteral(Formula literal)
        {
            if (literal is Variable)
                return new Not(literal);
            if (literal is Not)
                return (literal as Not).P;
            return null;
        }

        /// <summary>
        /// Determines whether a Cnf contains any unitary clause.
        /// </summary>
        /// <param name="cnf"></param>
        /// <returns>Returns the unit clause if it exists; null otherwise</returns>
        private Formula UnitClause(Cnf cnf)
        {
            foreach (var clause in cnf.Clauses)
                if (clause.Literals.Count == 1)
                    return clause.Literals.First();
            return null;
        }

        #endregion
   
        public override string ToString()
        {
            if (Clauses.Count > 0)
            {
                var result = "";
                foreach (var clausule in Clauses)
                {
                    var c = "";
                    foreach (var literal in clausule.Literals)
                        c += literal + ",";

                    result += "(" + c + ")";
                }
                return result;
            }

            return "Empty CNF";
        }
    }

}
