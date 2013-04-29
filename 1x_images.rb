#!/usr/bin/env ruby

# Recursively process all the @2x images to create the 1x images alongside them
# If you've used this script in the past, you may only want to process any @2x
# images that are newer than the 1x images you already have.

require 'find'
require 'fileutils'

mode = ARGV[0]

if mode !~ /^(all|newer)$/
  puts "usage:  #{$0}  all | newer"
  exit
end


Find.find(".") do |path|
  if FileTest.directory?(path)
    if File.basename(path)[0] == ?. and File.basename(path) != '.'
      Find.prune
    else
      next
    end
  else
    file_basename = File.basename(path)
    if file_basename =~ /@2x.png/
      dest_path = path.gsub(/@2x.png$/, ".png")

      should_process = false

      if mode == "all"
        should_process = true
      elsif mode == "newer"
        if ! File.exists?(dest_path)
          should_process = true
        elsif File.mtime(path) > File.mtime(dest_path)
          should_process = true
        end
      end

      if should_process
        cmd = "convert #{path} -resize 50% #{dest_path}"
        puts cmd
        `#{cmd}`
      end
    end
  end
end
