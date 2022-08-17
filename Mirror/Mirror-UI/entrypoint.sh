#!/bin/bash
npm start &
echo Backend Running
cd frontend
npm start 
echo Frontend Running