require 'fileutils'
require 'uuidtools'

# current version
puts %Q(> current version: #{File.read("version.txt")[/\d+\.\d+\.\d+/]})

# ask for version
puts "> enter version (format x.x.x)..."
version = STDIN.gets.chomp
fail "error: '#{version}' is not in the format 'x.x.x'." unless version.match(/^\d+\.\d+\.\d+$/)
version += '.0'

# generate required new guids
ProductCodeGuid = UUIDTools::UUID.random_create.to_s.upcase
PackageCodeGuid = UUIDTools::UUID.random_create.to_s.upcase

# update version and guids for setup.vdproj
['version.txt', 
 'Koffeinfrei.MinusShare/Properties/AssemblyInfo.cs'].each do |filename|
    puts "> updating '#{filename}'..."
    contents = File.read(filename)
    File.open(filename, 'w') do |file|
        file.puts contents
                    .gsub(/\d+\.\d+\.\d+\.\d+/, "#{version}")
    end
end

['Koffeinfrei.MinusShare.Setup/Koffeinfrei.MinusShare.Setup.vdproj'].each do |filename|
    puts "> updating '#{filename}'..."
    contents = File.read(filename)
    File.open(filename, 'w') do |file|
        file.puts contents
                    .gsub(/("ProductVersion" = "8:)\d+\.\d+\.\d+"/, "\\1#{version[/\d+\.\d+\.\d+/]}\"")
                    .gsub(/("ProductCode" = "8:){[^}]+}"/, "\\1{#{ProductCodeGuid}}\"")
                    .gsub(/("PackageCode" = "8:){[^}]+}"/, "\\1{#{PackageCodeGuid}}\"")
    end
end

# done
puts '> done.', "> increase the base library's version if the library was updated!!"