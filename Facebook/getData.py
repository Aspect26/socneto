# -*- coding: utf-8 -*-
"""
Created on Fri Feb 15 10:37:23 2019

@author: Petra Doubravova
"""
import urllib3
import facebook
import requests

token='EAAGLMcOLyewBAIXfQlWpGAsYdWmQHvII5XZCcK6Rt3ThaZC3kOvLSXHq3CDWSrm6eFStDfVTm6dgvWnCFs2zWimD7QnN7DndH9HV7slmO8uT1AfSQrDOXCZCw2fRJHihoKWCGGvPyrobXFayelKJ58aN98b7RQv9uGefaTZAfFEJktuvhVBv'
graph = facebook.GraphAPI(access_token=token, version = 2.9)

# Search for places near 1 Hacker Way in Menlo Park, California.
places = graph.search(type='place',
                      center='37.4845306,-122.1498183',
                      fields='name,location')

# Each given id maps to an object the contains the requested fields.
#for place in places['data']:
 #   print('%s %s' % (place['name'].encode(),place['location'].get('zip')))

############################################################
# Stránky ke kterým mám přístup
###########################################################    
pages = graph.request('/me/accounts')

####################feed pro všechny stránky + comments prvni urovne#############################
for page in pages['data']:
    posts = graph.request('/'+ page['id'] + '/feed')
    for post in posts['data']:
        comments = graph.request('/' + post['id'] + '/comments?comments')
    #print(page['name'] + ' ' + users)
    
    # comments of comments: 573130209785497/?fields=comments{message,comments{message},from,id}