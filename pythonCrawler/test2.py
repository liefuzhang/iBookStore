import requests    
import re    
from urllib.parse import urlparse    

html = requests.get("https://www.bookdepository.com/Claxton-Diary-Mark-Cocker/9781787331761?ref=grid-view")
content = html.content.decode('latin-1')

from bs4 import BeautifulSoup
soup = BeautifulSoup(content, 'html.parser')

book = soup.select('.item-info h1')[0]
author = soup.select('.author-info span')[0]
description = soup.select('.item-excerpt.trunc')[0]
print(book.text.strip())
print(author.text.strip())
print(description.text.strip())

