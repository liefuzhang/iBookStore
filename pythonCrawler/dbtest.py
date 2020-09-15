#Import pyodbc module using below command
import pyodbc as db
import requests    
import re    
import urllib.request
from urllib.parse import urlparse  
from bs4 import BeautifulSoup

class BookCrawler(object): 
    def __init__(self, starting_url, categoryId):    
        self.starting_url = starting_url    
        self.categoryId = categoryId

    def getLinks(self):
        html = requests.get(self.starting_url)
        content = html.content.decode('latin-1')

        soup = BeautifulSoup(content, 'html.parser')

        baseUrl = 'https://www.bookdepository.com/'
        bookUrls = []
        books = soup.select('h3.title a')
        for book in books:
            bookUrls.append(baseUrl + book["href"])

        return bookUrls

    def fetchBook(self, bookUrl):
        html = requests.get(bookUrl)
        content = html.content.decode('latin-1')

        soup = BeautifulSoup(content, 'html.parser')

        book = soup.select('.item-info h1')[0].text.strip()
        description = soup.select('.item-excerpt.trunc')[0].text.strip()
        if description.endswith('show more'):
            description = description[:-9]
        price = soup.select('.sale-price')[0].text.strip()
        if price.startswith('NZ$'):
            price = price[3:]
        pictureUrl = soup.select('.item-img-content img')[0]['src']
        author = soup.select('.author-info span')[0].text.strip()
        categoryId = self.categoryId
        availableStock = 100
        ISBN13 = soup.find_all(attrs={"itemprop": "isbn"})[0].text.strip()
        publicationDate = soup.find_all(attrs={"itemprop": "datePublished"})[0].text.strip()
        totalPage = soup.find_all(attrs={"itemprop": "numberOfPages"})[0].text.strip()
        if totalPage.endswith(' pages'):
            totalPage = totalPage[:-6]
        
        #Create connection string to connect DBTest database with windows authentication
        con = ''
        cur = con.cursor()

        #check if book already exists
        qry = 'SELECT 1 FROM dbo.CatalogItems WHERE ISBN13 = ' + ISBN13
        cur.execute(qry)
        
        row = cur.fetchone() #Fetch first row

        if row: #Fetch all rows using a while loop
            return
            
        #download image
        urllib.request.urlretrieve(pictureUrl, "bookImages/"+ISBN13+".jpg")

        #Insert into DB
        qry = '''INSERT INTO dbo.CatalogItems
                (Name, Description, Price, PictureUrl, Author, CategoryId, AvailableStock, ISBN13, PublicationDate, TotalPage)
                VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                '''
        param_values = [book, description, price, pictureUrl, author, categoryId, availableStock, ISBN13, publicationDate, totalPage]
        cur.execute(qry, param_values)
        
        print('{0} row inserted successfully.'.format(cur.rowcount))
        
        cur.commit() #Use this to commit the insert operation
        cur.close()
        con.close()

    def start(self):                     
        links = self.getLinks()
        for link in links:
            self.fetchBook(link)    

if __name__ == "__main__":                           
    crawler = BookCrawler("https://www.bookdepository.com/category/929/Economics", 9)        
    crawler.start()