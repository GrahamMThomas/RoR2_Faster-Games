import os
import re
import shutil  

# Get version increment
print("Which version number should increase?\n")
print("Major X.0.0 = 1")
print("Minor 0.X.0 = 2")
print("Patch 0.0.X = 3")
answer = input("Answer: ")

last_release_name = os.listdir('releases')[1]
v_nums = re.match(r"faster\-games_(\d+)\-(\d+)\-(\d+)", last_release_name).groups()

# Create new release name

new_v_nums = ''
if answer == "1":
    new_v_nums = [str(int(v_nums[0]) + 1),'0','0']
if answer == "2":
    new_v_nums = [v_nums[0],str(int(v_nums[1]) + 1),'0']
if answer == "3":
    new_v_nums = [v_nums[0],v_nums[1],str(int(v_nums[2]) + 1)]

new_release_name = "faster-games_"+'-'.join(new_v_nums)

# Copy Files
print("Creating " + new_release_name)
shutil.copytree("releases/"+last_release_name, "releases/"+new_release_name)
shutil.copyfile("bin/Debug/netstandard2.0/FasterGames.dll", "releases/"+new_release_name+"/FasterGames.dll")
shutil.copyfile("README.md", "releases/"+new_release_name+"/README.md")
print("Files copied.")

# Updates manifest
# Read in the file
with open("releases/"+new_release_name+"/manifest.json", 'r') as file :
  manifest = file.read()

# Replace the target string
manifest = manifest.replace('.'.join(v_nums), '.'.join(new_v_nums))

# Write the file out again
with open("releases/"+new_release_name+"/manifest.json", 'w') as file:
  file.write(manifest)


# Make Archive
shutil.make_archive("releases/"+new_release_name, 'zip', "releases/"+new_release_name)
print("Packaged.")