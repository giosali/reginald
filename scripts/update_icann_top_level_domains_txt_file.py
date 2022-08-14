import pathlib

import requests


USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36"
URL = "https://data.iana.org/TLD/tlds-alpha-by-domain.txt"
REPO_NAME = "Reginald"
FILE_NAME_PARENT_NAME = "Resources"
FILE_NAME = "TopLevelDomains.txt"


def main():
    s: requests.Session = requests.Session()
    s.headers.update({"user-agent": USER_AGENT})
    with s.get(URL) as r:
        r.raise_for_status()

    top_level_domains: list[str] = [
        line + "\n"
        for line in r.text.splitlines()[:-1]
        if not line.startswith("# Version")
    ]
    top_level_domains.append(r.text.splitlines()[-1])
    file: pathlib.Path = get_tld_txt_file()
    with open(file, "w") as f:
        f.writelines(top_level_domains)


def get_tld_txt_file() -> pathlib.Path:
    p: pathlib.Path = pathlib.Path(__file__)
    repo_dir: pathlib.Path = None
    while True:
        if p.name == REPO_NAME:
            repo_dir = p
            break
        p = p.parent

    main_dir: pathlib.Path = find_item_in_dir(repo_dir, REPO_NAME)
    resources_dir: pathlib.Path = find_item_in_dir(main_dir, FILE_NAME_PARENT_NAME)
    file: pathlib.Path = find_item_in_dir(resources_dir, FILE_NAME)
    return file


def find_item_in_dir(dir: pathlib.Path, name: str) -> pathlib.Path:
    for item in dir.iterdir():
        if item.name == name:
            return item


if __name__ == "__main__":
    main()
