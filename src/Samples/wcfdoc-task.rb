def wcfdoc(*args, &block)
	task = lambda { |*args|
		task = WcfDoc.new
		block.call(task)
		task.run
	}
	Rake::Task.define_task(*args, &task)
end

class WcfDoc

    attr_accessor :wcf_doc_path, :assembly_path, :output_path, :stylesheet_path, :web_root_path,
                  :web_config_path, :xml_comments_path, :service_type
    
    def initialize()
		@wcf_doc_path = "C:/Program Files (x86)/Wcf Doc/WcfDoc.exe"
    end
    
    def run()
        
		wcfdoc = Array.new
		
        wcfdoc << "\"#{@wcf_doc_path}\""
        wcfdoc << "/Assemblies:\"#{@assembly_path}\""
		wcfdoc << "/Output:\"#{@output_path}\""
		
		if @stylesheet_path != nil then wcfdoc << "/Stylesheet:\"#{@stylesheet_path}\"" end
		if @web_root_path != nil then wcfdoc << "/WebsitePath:\"#{@web_root_path}\"" end
		if @web_config_path != nil then wcfdoc << "/Config:\"#{@web_config_path}\"" end
		if @xml_comments_path != nil then wcfdoc << "/XmlComments:\"#{@xml_comments_path}\"" end
		if @service_type != nil then wcfdoc << "/ServiceType:\"#{@service_type}\"" end

        system(wcfdoc.join(" ")) 
    end
    
end

    