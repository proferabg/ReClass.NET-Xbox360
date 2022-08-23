using ReClassNET.CodeGenerator;
using ReClassNET.Logger;
using ReClassNET.Nodes;

namespace Xbox360Plugin.Pointer32
{
    public class Pointer32CodeGenerator : CustomCppCodeGenerator
    {
        public override bool CanHandle(BaseNode node)
        {
            return node is Pointer32Node;
        }

        public override BaseNode TransformNode(BaseNode node)
        {
            return node;
        }

        public override string GetTypeDefinition(BaseNode node, GetTypeDefinitionFunc defaultGetTypeDefinitionFunc, ResolveWrappedTypeFunc defaultResolveWrappedTypeFunc, ILogger logger)
        {
            return $"Pointer32<class {((Pointer32Node)node).InnerNode.Name}>";
        }
    }
}
