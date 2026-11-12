import os
import argparse
from pathlib import Path

def get_directory_structure(root_dir, max_depth=None, exclude_dirs=None):
    """
    获取目录结构
    
    Args:
        root_dir: 根目录路径
        max_depth: 最大深度，None表示不限制
        exclude_dirs: 要排除的目录列表
    
    Returns:
        目录结构的字符串表示
    """
    if exclude_dirs is None:
        exclude_dirs = ['.git', '__pycache__', '.idea', 'node_modules']
    
    def build_tree(directory, prefix="", depth=0):
        if max_depth is not None and depth > max_depth:
            return ""
        
        try:
            entries = sorted(os.listdir(directory))
        except PermissionError:
            return f"{prefix}📁 {os.path.basename(directory)} (权限拒绝)\n"
        
        # 过滤出目录并排除指定目录
        dirs = [entry for entry in entries 
                if os.path.isdir(os.path.join(directory, entry)) 
                and entry not in exclude_dirs]
        
        tree_str = ""
        if depth == 0:
            tree_str += f"# 目录结构: {os.path.basename(directory)}\n\n"
            tree_str += "```\n"
            tree_str += f"{directory}  \n"
        else:
            tree_str += f"{prefix}📁 {os.path.basename(directory)}  \n"
        
        for i, dir_name in enumerate(dirs):
            dir_path = os.path.join(directory, dir_name)
            is_last = i == len(dirs) - 1
            
            if depth == 0:
                connector = "&emsp;&emsp;├──" if not is_last else "&emsp;&emsp;└──"
            else:
                connector = " │&emsp;" if not is_last else "── "
            
            tree_str += build_tree(dir_path, prefix + connector, depth + 1)
        
        return tree_str
    
    result = build_tree(root_dir)
    return result + "```\n"

def save_to_markdown(content, output_file="directory_structure.md"):
    """
    将目录结构保存为Markdown文件
    
    Args:
        content: 目录结构内容
        output_file: 输出文件名
    """
    try:
        with open(output_file, 'w', encoding='utf-8') as f:
            f.write(content)
        print(f"目录结构已保存到: {output_file}")
    except Exception as e:
        print(f"保存文件时出错: {e}")

def main():
    parser = argparse.ArgumentParser(description='生成目录结构的Markdown文件')
    parser.add_argument('--dir', '-d', default='.', 
                       help='要扫描的目录路径 (默认: 当前目录)')
    parser.add_argument('--output', '-o', default='directory_structure.md',
                       help='输出文件名 (默认: directory_structure.md)')
    parser.add_argument('--max-depth', '-m', type=int, default=None,
                       help='最大深度 (默认: 不限制)')
    parser.add_argument('--exclude', '-e', nargs='+', 
                       default=['.git', '__pycache__', '.idea', 'node_modules'],
                       help='要排除的目录列表')
    
    args = parser.parse_args()
    
    # 检查目录是否存在
    if not os.path.exists(args.dir):
        print(f"目录不存在: {args.dir}")
        return
    
    if not os.path.isdir(args.dir):
        print(f"路径不是目录: {args.dir}")
        return
    
    print(f"正在扫描目录: {os.path.abspath(args.dir)}")
    print(f"输出文件: {args.output}")
    if args.max_depth:
        print(f"最大深度: {args.max_depth}")
    print(f"排除目录: {', '.join(args.exclude)}")
    print("-" * 50)
    
    # 获取目录结构
    directory_structure = get_directory_structure(
        args.dir, 
        max_depth=args.max_depth, 
        exclude_dirs=args.exclude
    )
    
    # 保存为Markdown文件
    save_to_markdown(directory_structure, args.output)
    
    # 在控制台显示结果
    print("\n生成的目录结构:")
    print(directory_structure)

if __name__ == "__main__":
    main()