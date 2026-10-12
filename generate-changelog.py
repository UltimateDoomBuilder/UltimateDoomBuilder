import argparse
import html
from git import Repo, GitCommandError

def generate_xml_changelog(commits):
    num_commits = len(commits)

    text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"
    text += "<log>\n"

    for index, commit in enumerate(commits):
        text += f'  <logentry commit="{commit.hexsha[:8]}" revision="{num_commits - index}">\n'
        text += f'    <author>{commit.author.name}</author>\n'
        text += f'    <date>{commit.committed_datetime.isoformat()}</date>\n'
        text += f'    <msg>{html.escape(commit.message.strip())}</msg>\n'
        text += f'  </logentry>\n'

    text += "</log>\n"
    return text

def generate_text_changelog(commits):
    num_commits = len(commits)

    text = ""
    for index, commit in enumerate(commits[:3]):
        text += f'R{num_commits - index} | {commit.hexsha[:8]} | {commit.author.name} | {commit.committed_datetime.strftime("%Y-%m-%d, %H:%M")}\n'
        text += '-' * 80 + "\n"
        text += f'{commit.message.strip()}\n'
        text += '\n'

    return text

if __name__ == "__main__":
    argparser = argparse.ArgumentParser(description='Generate changelog from git commits')
    argparser.add_argument('--branch', required=True, help='Specify the branch to generate the changelog from')
    argparser.add_argument('--format', required=True, help='Specify the format of the changelog (text or xml)', choices=['text', 'xml'])
    args = argparser.parse_args()

    try:
        repo = Repo(".")
        commits = list(repo.iter_commits(args.branch))
    except GitCommandError as e:
        print(f"Error: {e}")
        exit(1)

    if args.format == 'xml':
        print(generate_xml_changelog(commits))
    else:
        print(generate_text_changelog(commits))