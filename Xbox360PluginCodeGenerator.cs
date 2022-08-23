using ReClassNET.CodeGenerator;
using ReClassNET.Logger;
using ReClassNET.Nodes;
using Xbox360Plugin.TextPointers;

namespace Xbox360Plugin
{
    public class Xbox360PluginCodeGenerator : CustomCppCodeGenerator
    {
        public override bool CanHandle(BaseNode node)
        {
            return node is Pointer32Node ||
                   node is Utf8TextPtr32Node ||
                   node is Utf16TextPtr32Node;
        }

        public override BaseNode TransformNode(BaseNode node)
        {
            return node;
        }

        public override string GetTypeDefinition(BaseNode node, GetTypeDefinitionFunc defaultGetTypeDefinitionFunc, ResolveWrappedTypeFunc defaultResolveWrappedTypeFunc, ILogger logger)
        {
            if(node is Pointer32Node)
                return $"{((ClassInstanceNode)((Pointer32Node)node).InnerNode).InnerNode.Name}*";

            if (node is Utf8TextPtr32Node)
                return "char *";

            if (node is Utf16TextPtr32Node)
                return "wchar_t *";

            return "";
        }
    }
}
