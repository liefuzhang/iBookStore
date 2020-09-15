import pyodbc as db
import requests    
import re    
import urllib.request
from urllib.parse import urlparse  
from bs4 import BeautifulSoup

html = requests.get('https://www.bookdepository.com/category/215/Autobiography-General/browse/viewmode/all')
content = html.content.decode('latin-1')

soup = BeautifulSoup(content, 'html.parser')

baseUrl = 'https://www.bookdepository.com/'
bookUrls = []
books = soup.select('h3.title a')
for book in books:
    bookUrls.append(baseUrl + book["href"])

