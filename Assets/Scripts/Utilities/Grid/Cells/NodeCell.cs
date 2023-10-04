
namespace Utilities.AI
{
    public class NodeCell : GridCell<int>
    {
        public int GCost;
        public int HCost;
        public int FCost => value;
        public bool IsWalkable = true;

        public NodeCell Parent;
        public override string ToString()
        {
            return $"{x},{y}";
        }

        public void CalculateFCost()
        {
            value = GCost + HCost;
        }
    }
}