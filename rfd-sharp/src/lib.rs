use rnet::net;

rnet::root!();


#[net]
fn save_file(directory: String) -> Option<String> {
    open_file_inner_net(&directory, Vec::new(), true)
}

#[net]
fn save_file_with_filter(
    directory: String, 
    filters: Vec<String>
) -> Option<String> {
    open_file_inner_net(&directory, filters, true)
}

#[net]
fn open_file(directory: String) -> Option<String> {
    open_file_inner_net(&directory, Vec::new(), false)
}

#[net]
fn open_file_with_filter(
    directory: String, 
    filters: Vec<String>
) -> Option<String> {
    open_file_inner_net(&directory, filters, false)
}

fn open_file_inner_net(directory: &str, filters: Vec<String>, is_save: bool) 
-> Option<String> {
    let mut rfd = rfd::FileDialog::new()
        .set_directory(directory);
    let filters = filters.iter().map(|x| x.as_str()).collect::<Vec<&str>>();
    
    if !filters.is_empty() {
        rfd = rfd.add_filter("Supported", &filters);
    }
    
    let files = if is_save {
        rfd.save_file()
    } else {
        rfd.pick_file()
    };

    
    if let Some(files) = files {
        let Some(files) = files.to_str() else { 
            return None
        };
        Some(files.into())
    } else {
        None
    }
}
