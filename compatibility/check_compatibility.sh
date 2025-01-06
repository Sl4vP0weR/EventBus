baseline_packages_dir="./"
packages_dir="../packages/"

exit_code=0

for baseline_package in "$baseline_packages_dir"**/*.nupkg 
do
    package_id=$(basename "$baseline_package" | cut -d. -f1)
    version=$(basename "$baseline_package" | cut -d. -f2-4)

    echo -e "\033[36mCompatibility check for \033[35m$package_id v$version\033[33m"
    echo
    suppression_file="$package_id.suppress.xml"
    [ -f "$suppression_file" ] || suppression_file="default.suppress.xml"

    package=$(find $packages_dir -name "$package_id*.nupkg" | sort -V | tail -n 1)

    if [ -f "$package" ]; then
        output=$(
        apicompat \
            package "$package" \
            --baseline-package "$baseline_package" \
            --noWarn "CP0003" \
            --suppression-file "$suppression_file"
        )

        if [ -n "$output" ]; then
            echo -e "\033[32m$output"
            echo -e "\033[32mCompatibility check successful for \033[35m$package_id"
        else
            echo -e "\033[31m$output"
            echo -e "\033[31mCompatibility errors detected for \033[35m$package_id"
            exit_code=1
        fi
    fi
    echo -e "\033[0m"
done

exit $exit_code
