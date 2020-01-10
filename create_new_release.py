import os
import re
import shutil  

print("Which version number should increase?\n")
print("Major X.0.0 = 1")
print("Minor 0.X.0 = 2")
print("Patch 0.0.X = 3")
answer = input("Answer: ")

last_release_name = os.listdir('releases')[0]
v_nums = re.match(r"faster\-games_(\d+)\-(\d+)\-(\d+)", last_release_name).groups()

new_release_name = "faster-games_"
if answer == "1":
    new_release_name += str((int(v_nums[0]) + 1)) + "-0-0"
if answer == "2":
    new_release_name += v_nums[0] + "-" + str((int(v_nums[1]) + 1)) + "-0"
if answer == "3":
    new_release_name += v_nums[0] + "-" + v_nums[1] + "-" + str((int(v_nums[2]) + 1))

print("Creating " + new_release_name)
shutil.copytree("releases/"+last_release_name, "releases/"+new_release_name)
shutil.copyfile("bin/Debug/netstandard2.0/FasterGames.dll", "releases/"+new_release_name+"/FasterGames.dll")
shutil.copyfile("README.md", "releases/"+new_release_name+"/README.md")
print("Files copied.")

shutil.make_archive("releases/"+new_release_name, 'zip', "releases/"+new_release_name)
print("Packaged.")